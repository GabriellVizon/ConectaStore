using System.Text;
using ConectaStore.API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
string conexao = builder.Configuration.GetConnectionString("Conexao");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(conexao)
    );

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("LiberarFront",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
       Title = "ConectaStore.API",
       Version = "v1",
       Description = "API para gerenciamento de produtos da ConectaStore",
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT:"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Garante que o banco de dados exista ao executar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    // Cria tabelas novas se não existirem (para bancos já existentes)
    await db.Database.ExecuteSqlRawAsync(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Vendas')
        BEGIN
            CREATE TABLE [Vendas] (
                [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [Data] DATETIME2 NOT NULL,
                [Cliente] NVARCHAR(200) NOT NULL,
                [Total] DECIMAL(10,2) NOT NULL,
                [Status] NVARCHAR(50) NOT NULL
            );
        END
    ");
    await db.Database.ExecuteSqlRawAsync(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'ItensVenda')
        BEGIN
            CREATE TABLE [ItensVenda] (
                [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [VendaId] INT NOT NULL,
                [ProdutoId] INT NOT NULL,
                [Quantidade] INT NOT NULL,
                [PrecoUnitario] DECIMAL(10,2) NOT NULL,
                CONSTRAINT [FK_ItensVenda_Vendas] FOREIGN KEY ([VendaId]) REFERENCES [Vendas]([Id]) ON DELETE CASCADE,
                CONSTRAINT [FK_ItensVenda_Produtos] FOREIGN KEY ([ProdutoId]) REFERENCES [Produtos]([Id])
            );
        END
    ");
    await db.Database.ExecuteSqlRawAsync(@"
        IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Usuarios')
        BEGIN
            CREATE TABLE [Usuarios] (
                [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                [Nome] NVARCHAR(200) NOT NULL,
                [Email] NVARCHAR(200) NOT NULL,
                [SenhaHash] NVARCHAR(500) NOT NULL,
                [DataCadastro] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
            );
        END
    ");

    // Seed categorias se banco estiver vazio
    if (!await db.Categorias.AnyAsync())
    {
        db.Categorias.AddRange(
            new ConectaStore.API.Models.Categoria { Nome = "Smartphones", Cor = "#FF5733" },
            new ConectaStore.API.Models.Categoria { Nome = "Notebooks", Cor = "#33FF57" },
            new ConectaStore.API.Models.Categoria { Nome = "SmartWatches", Cor = "#3357FF" },
            new ConectaStore.API.Models.Categoria { Nome = "Fones de Ouvido", Cor = "#FF33A1" },
            new ConectaStore.API.Models.Categoria { Nome = "Tablets", Cor = "#A855F7" },
            new ConectaStore.API.Models.Categoria { Nome = "Acessórios", Cor = "#F59E0B" },
            new ConectaStore.API.Models.Categoria { Nome = "Gaming", Cor = "#10B981" },
            new ConectaStore.API.Models.Categoria { Nome = "Áudio", Cor = "#EC4899" }
        );
        await db.SaveChangesAsync();
    }

    var adminEmail = builder.Configuration["Admin:Email"] ?? "admin@conecta.com";
    if (!await db.Usuarios.AnyAsync(u => u.Email == adminEmail))
    {
        byte[] salt = System.Security.Cryptography.RandomNumberGenerator.GetBytes(16);
        byte[] hash = System.Security.Cryptography.Rfc2898DeriveBytes.Pbkdf2("admin123", salt, 100000, System.Security.Cryptography.HashAlgorithmName.SHA256, 32);
        db.Usuarios.Add(new ConectaStore.API.Models.Usuario
        {
            Nome = "Administrador",
            Email = adminEmail,
            SenhaHash = Convert.ToHexString(salt) + ":" + Convert.ToHexString(hash),
            DataCadastro = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    var smartphones = await db.Categorias.FirstAsync(c => c.Nome == "Smartphones");
    var notebooks = await db.Categorias.FirstAsync(c => c.Nome == "Notebooks");
    var fones = await db.Categorias.FirstAsync(c => c.Nome == "Fones de Ouvido");
    var tablets = await db.Categorias.FirstAsync(c => c.Nome == "Tablets");
    var acessorios = await db.Categorias.FirstAsync(c => c.Nome == "Acessórios");
    var gaming = await db.Categorias.FirstAsync(c => c.Nome == "Gaming");
    var audio = await db.Categorias.FirstAsync(c => c.Nome == "Áudio");

    if (!await db.Produtos.AnyAsync())
    {
        db.Produtos.AddRange(
            new ConectaStore.API.Models.Produto { CategoriaId = smartphones.Id, Nome = "iPhone 17 Pro", Descricao = "O iPhone 17 Pro é o mais recente lançamento da Apple, trazendo um design elegante e recursos avançados. Com uma tela Super Retina XDR de 6,1 polegadas, o dispositivo oferece uma experiência visual imersiva. Equipado com o chip A17 Bionic, o iPhone 17 Pro proporciona desempenho excepcional e eficiência energética. O sistema de câmera tripla inclui uma lente principal de 12 MP, uma lente ultra-angular de 12 MP e uma lente telefoto de 12 MP, permitindo fotos e vídeos de alta qualidade.", ValorCusto = 1000m, ValorVenda = 10000.50m, Qtde = 5, Destaque = true, Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-17-pro-gold-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800" },
            new ConectaStore.API.Models.Produto { CategoriaId = notebooks.Id, Nome = "MacBook Air M2", Descricao = "O MacBook Air é o notebook mais leve e portátil da Apple. Com o chip M2, oferece desempenho excepcional e eficiência energética. A tela Retina de 13,6 polegadas proporciona uma experiência visual imersiva. Até 18 horas de autonomia.", ValorCusto = 2000m, ValorVenda = 20000.99m, Qtde = 3, Destaque = true, Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/macbook-air-m2-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800" },
            new ConectaStore.API.Models.Produto { CategoriaId = smartphones.Id, Nome = "Galaxy S24 Ultra", Descricao = "Smartphone premium Samsung com tela Dynamic AMOLED 2X de 6.8 polegadas, processador Snapdragon 8 Gen 3, câmera de 200MP, S-PEN integrada.", ValorCusto = 2500m, ValorVenda = 11000.00m, Qtde = 7, Destaque = true, Foto = "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-s24-ultra/bu_s24_ultra_titanium_gray.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = notebooks.Id, Nome = "Galaxy Book4 Pro", Descricao = "Notebook premium Samsung com tela AMOLED 3K de 16 polegadas, processador Intel Core Ultra 7, 16GB RAM, SSD 512GB.", ValorCusto = 3000m, ValorVenda = 15000.00m, Qtde = 2, Destaque = true, Foto = "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-book4-pro/bu_book4_pro_platinum_silver.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = fones.Id, Nome = "Galaxy Buds3 Pro", Descricao = "Fones de ouvido True Wireless com cancelamento ativo de ruído adaptativo, som 24-bit Hi-Fi, bateria de 6h + 18h estojo.", ValorCusto = 500m, ValorVenda = 2200.00m, Qtde = 10, Destaque = false, Foto = "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-buds3-pro/bu_buds3_pro_silver.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = tablets.Id, Nome = "Samsung Galaxy Tab S9", Descricao = "Tablet premium com tela Dynamic AMOLED 2X de 11 polegadas, processador Snapdragon 8 Gen 2, caneta S-PEN inclusa, resistência à água IP68.", ValorCusto = 1800m, ValorVenda = 8500.00m, Qtde = 6, Destaque = true, Foto = "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-tab-s9/bu_tab_s9_lunar_silver.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = tablets.Id, Nome = "iPad Air M2", Descricao = "iPad Air com chip M2, tela Liquid Retina de 11 polegadas, compatível com Apple Pencil e Magic Keyboard. Ideal para produtividade e criatividade.", ValorCusto = 2200m, ValorVenda = 9200.00m, Qtde = 4, Destaque = false, Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/ipad-air-m2-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800" },
            new ConectaStore.API.Models.Produto { CategoriaId = acessorios.Id, Nome = "Carregador Turbo 65W", Descricao = "Carregador USB-C PD 65W compacto, compatível com notebooks e smartphones. Carregamento rápido GaN, proteção contra sobrecarga.", ValorCusto = 45m, ValorVenda = 189.90m, Qtde = 25, Destaque = false, Foto = "https://m.media-amazon.com/images/I/51L3tG7tJTL._AC_SX679_.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = acessorios.Id, Nome = "Capa para Notebook 15\"", Descricao = "Capa protetora universal para notebooks de até 15.6 polegadas. Material resistente a impactos, interior acolchoado, alça de transporte.", ValorCusto = 35m, ValorVenda = 149.90m, Qtde = 18, Destaque = false, Foto = "https://m.media-amazon.com/images/I/71b8J1fVqOL._AC_SX679_.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = gaming.Id, Nome = "Mouse Gamer RGB", Descricao = "Mouse gamer com sensor óptico de 16000 DPI, 8 botões programáveis, iluminação RGB personalizável, cabo trançado USB.", ValorCusto = 60m, ValorVenda = 249.90m, Qtde = 15, Destaque = true, Foto = "https://m.media-amazon.com/images/I/61Hn4TCjtDL._AC_SX679_.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = gaming.Id, Nome = "Teclado Mecânico Gamer", Descricao = "Teclado mecânico RGB com switches azuis, estrutura em alumínio, teclas PBT double-shot, anti-ghosting, USB-C destacável.", ValorCusto = 110m, ValorVenda = 449.90m, Qtde = 10, Destaque = false, Foto = "https://m.media-amazon.com/images/I/61pimnpIadL._AC_SX679_.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = gaming.Id, Nome = "Headset Gamer 7.1", Descricao = "Headset gamer com som surround 7.1 virtual, drivers de 50mm, microfone com cancelamento de ruído, almofadas em couro respirável.", ValorCusto = 90m, ValorVenda = 379.90m, Qtde = 7, Destaque = true, Foto = "https://m.media-amazon.com/images/I/61CGHv6kmWL._AC_SX679_.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = audio.Id, Nome = "Caixa de Som Bluetooth", Descricao = "Caixa de som portátil Bluetooth 5.3, 30W RMS, graves potentes, resistência à água IPX7, bateria de 12 horas.", ValorCusto = 80m, ValorVenda = 329.90m, Qtde = 14, Destaque = true, Foto = "https://m.media-amazon.com/images/I/71HdLDJEEUL._AC_SX679_.jpg" },
            new ConectaStore.API.Models.Produto { CategoriaId = audio.Id, Nome = "Soundbar 2.1", Descricao = "Soundbar 2.1 canais com subwoofer wireless, 200W RMS, Bluetooth 5.0, entradas HDMI ARC e óptica, controle remoto incluso.", ValorCusto = 200m, ValorVenda = 899.90m, Qtde = 5, Destaque = false, Foto = "https://m.media-amazon.com/images/I/51YmunJoecL._AC_SX679_.jpg" }
        );
        await db.SaveChangesAsync();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ConectaStore.API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseCors("LiberarFront");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
