using ConectaStore.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ConectaStore.API.Data;

public class AppDbContext : DbContext
{
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        SeedCategoria(modelBuilder);
        SeedProduto(modelBuilder);
    }

    private static void SeedCategoria(ModelBuilder builder)

    {
        List<Categoria> categorias = [
          new() { Id = 1, Nome = "Smartphones", Cor = "#FF5733" },
          new() { Id = 2, Nome = "Notebooks", Cor = "#33FF57" },
          new() { Id = 3, Nome = "SmartWatches", Cor = "#3357FF" },
          new() { Id = 4, Nome = "Fones de Ouvido", Cor = "#FF33A1" },
          new() { Id = 5, Nome = "Tablets", Cor = "#A855F7" },
          new() { Id = 6, Nome = "Acessórios", Cor = "#F59E0B" },
          new() { Id = 7, Nome = "Gaming", Cor = "#10B981" },
          new() { Id = 8, Nome = "Áudio", Cor = "#EC4899" }
        ];
        builder.Entity<Categoria>().HasData(categorias);
    }

    private static void SeedProduto(ModelBuilder builder)
    {
        List<Produto> produtos = [
            new() { Id = 1, CategoriaId = 1, Nome = "iPhone 17 Pro", Descricao = "O iPhone 17 Pro é o mais recente lançamento da Apple, trazendo um design elegante e recursos avançados. Com uma tela Super Retina XDR de 6,1 polegadas, o dispositivo oferece uma experiência visual imersiva. Equipado com o chip A17 Bionic, o iPhone 17 Pro proporciona desempenho excepcional e eficiência energética. O sistema de câmera tripla inclui uma lente principal de 12 MP, uma lente ultra-angular de 12 MP e uma lente telefoto de 12 MP, permitindo fotos e vídeos de alta qualidade.", ValorCusto = 1000m, ValorVenda = 10000.50m, Qtde = 5, Destaque = true, Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-17-pro-gold-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800" },
            new() { Id = 2, CategoriaId = 2, Nome = "MacBook Air M2", Descricao = "O MacBook Air é o notebook mais leve e portátil da Apple. Com o chip M2, oferece desempenho excepcional e eficiência energética. A tela Retina de 13,6 polegadas proporciona uma experiência visual imersiva. Até 18 horas de autonomia.", ValorCusto = 2000m, ValorVenda = 20000.99m, Qtde = 3, Destaque = true, Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/macbook-air-m2-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800" },
            new() { Id = 3, CategoriaId = 3, Nome = "Apple Watch Series 9", Descricao = "Smartwatch mais avançado da Apple. Tela Retina sempre ativa, monitoramento de frequência cardíaca, rastreamento de atividades, detecção de quedas. Resistente à água.", ValorCusto = 300m, ValorVenda = 2500.00m, Qtde = 8, Destaque = true, Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/apple-watch-series-9-gps-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800" },
            new() { Id = 4, CategoriaId = 4, Nome = "AirPods Pro 2", Descricao = "Fones de ouvido sem fio premium com cancelamento ativo de ruído, modo transparência, resistentes à água e suor. Carregamento sem fio incluso.", ValorCusto = 700m, ValorVenda = 5600.00m, Qtde = 12, Destaque = true, Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MWP22?wid=940&hei=1112&fmt=png-alpha&.v=1700792800" },
            new() { Id = 5, CategoriaId = 5, Nome = "Samsung Galaxy Tab S9", Descricao = "Tablet premium com tela Dynamic AMOLED 2X de 11 polegadas, processador Snapdragon 8 Gen 2, caneta S-PEN inclusa, resistência à água IP68.", ValorCusto = 1800m, ValorVenda = 8500.00m, Qtde = 6, Destaque = true, Foto = "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-tab-s9/bu_tab_s9_lunar_silver.jpg" },
            new() { Id = 6, CategoriaId = 5, Nome = "iPad Air M2", Descricao = "iPad Air com chip M2, tela Liquid Retina de 11 polegadas, compatível com Apple Pencil e Magic Keyboard. Ideal para produtividade e criatividade.", ValorCusto = 2200m, ValorVenda = 9200.00m, Qtde = 4, Destaque = false, Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/ipad-air-m2-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800" },
            new() { Id = 7, CategoriaId = 6, Nome = "Carregador Turbo 65W", Descricao = "Carregador USB-C PD 65W compacto, compatível com notebooks e smartphones. Carregamento rápido GaN, proteção contra sobrecarga.", ValorCusto = 45m, ValorVenda = 189.90m, Qtde = 25, Destaque = false, Foto = "https://m.media-amazon.com/images/I/51L3tG7tJTL._AC_SX679_.jpg" },
            new() { Id = 8, CategoriaId = 6, Nome = "Capa para Notebook 15\"", Descricao = "Capa protetora universal para notebooks de até 15.6 polegadas. Material resistente a impactos, interior acolchoado, alça de transporte.", ValorCusto = 35m, ValorVenda = 149.90m, Qtde = 18, Destaque = false, Foto = "https://m.media-amazon.com/images/I/71b8J1fVqOL._AC_SX679_.jpg" },
            new() { Id = 9, CategoriaId = 7, Nome = "Mouse Gamer RGB", Descricao = "Mouse gamer com sensor óptico de 16000 DPI, 8 botões programáveis, iluminação RGB personalizável, cabo trançado USB.", ValorCusto = 60m, ValorVenda = 249.90m, Qtde = 15, Destaque = true, Foto = "https://m.media-amazon.com/images/I/61Hn4TCjtDL._AC_SX679_.jpg" },
            new() { Id = 10, CategoriaId = 7, Nome = "Teclado Mecânico Gamer", Descricao = "Teclado mecânico RGB com switches azuis, estrutura em alumínio, teclas PBT double-shot, anti-ghosting, USB-C destacável.", ValorCusto = 110m, ValorVenda = 449.90m, Qtde = 10, Destaque = false, Foto = "https://m.media-amazon.com/images/I/71b8J1fVqOL._AC_SX679_.jpg" },
            new() { Id = 11, CategoriaId = 7, Nome = "Headset Gamer 7.1", Descricao = "Headset gamer com som surround 7.1 virtual, drivers de 50mm, microfone com cancelamento de ruído, almofadas em couro respirável.", ValorCusto = 90m, ValorVenda = 379.90m, Qtde = 7, Destaque = true, Foto = "https://m.media-amazon.com/images/I/61CGHv6kmWL._AC_SX679_.jpg" },
            new() { Id = 12, CategoriaId = 8, Nome = "Caixa de Som Bluetooth", Descricao = "Caixa de som portátil Bluetooth 5.3, 30W RMS, graves potentes, resistência à água IPX7, bateria de 12 horas.", ValorCusto = 80m, ValorVenda = 329.90m, Qtde = 14, Destaque = true, Foto = "https://m.media-amazon.com/images/I/71b8J1fVqOL._AC_SX679_.jpg" },
            new() { Id = 13, CategoriaId = 8, Nome = "Soundbar 2.1", Descricao = "Soundbar 2.1 canais com subwoofer wireless, 200W RMS, Bluetooth 5.0, entradas HDMI ARC e óptica, controle remoto incluso.", ValorCusto = 200m, ValorVenda = 899.90m, Qtde = 5, Destaque = false, Foto = "https://m.media-amazon.com/images/I/61CGHv6kmWL._AC_SX679_.jpg" },
            new() { Id = 14, CategoriaId = 1, Nome = "Galaxy S24 Ultra", Descricao = "Smartphone premium Samsung com tela Dynamic AMOLED 2X de 6.8 polegadas, processador Snapdragon 8 Gen 3, câmera de 200MP, S-PEN integrada.", ValorCusto = 2500m, ValorVenda = 11000.00m, Qtde = 7, Destaque = true, Foto = "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-s24-ultra/bu_s24_ultra_titanium_gray.jpg" },
            new() { Id = 15, CategoriaId = 2, Nome = "Galaxy Book4 Pro", Descricao = "Notebook premium Samsung com tela AMOLED 3K de 16 polegadas, processador Intel Core Ultra 7, 16GB RAM, SSD 512GB.", ValorCusto = 3000m, ValorVenda = 15000.00m, Qtde = 2, Destaque = true, Foto = "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-book4-pro/bu_book4_pro_platinum_silver.jpg" },
            new() { Id = 16, CategoriaId = 4, Nome = "Galaxy Buds3 Pro", Descricao = "Fones de ouvido True Wireless com cancelamento ativo de ruído adaptativo, som 24-bit Hi-Fi, bateria de 6h + 18h estojo.", ValorCusto = 500m, ValorVenda = 2200.00m, Qtde = 10, Destaque = false, Foto = "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-buds3-pro/bu_buds3_pro_silver.jpg" }
        ];
            builder.Entity<Produto>().HasData(produtos);
    }
}
