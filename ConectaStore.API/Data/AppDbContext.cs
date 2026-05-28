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
        new() { Id = 4, Nome = "Fones de Ouvido", Cor = "#FF33A1" }
        ];
        builder.Entity<Categoria>().HasData(categorias);
    }

    private static void SeedProduto(ModelBuilder builder)
    {
        List<Produto> produtos = [
            new() { 
            Id = 1,
            CategoriaId = 1,
            Nome = "iPhone 17 Pro",
            Descricao = "O iPhone 17 Pro é o mais recente lançamento da Apple, trazendo um design elegante e recursos avançados. Com uma tela Super Retina XDR de 6,1 polegadas, o dispositivo oferece uma experiência visual imersiva. Equipado com o chip A17 Bionic, o iPhone 17 Pro proporciona desempenho excepcional e eficiência energética. O sistema de câmera tripla inclui uma lente principal de 12 MP, uma lente ultra-angular de 12 MP e uma lente telefoto de 12 MP, permitindo fotos e vídeos de alta qualidade. Além disso, o iPhone 17 Pro suporta carregamento rápido e possui resistência à água e poeira IP68.",
            ValorCusto = 1000m,
            ValorVenda = 10000.50m,
            Qtde = 0,
            Destaque = true,
            Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-17-pro-gold-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800",
            },
            new() {
            Id = 2,
            CategoriaId = 2,
            Nome = "MacBook Air",
            Descricao = "O MacBook Air é o notebook mais leve e portátil da Apple, ideal para uso diário e produtividade. Com o chip M2, oferece desempenho excepcional e eficiência energética. A tela Retina de 13,6 polegadas proporciona uma experiência visual imersiva. O MacBook Air é leve, silencioso e oferece até 18 horas de autonomia.",
            ValorCusto = 2000m,
            ValorVenda = 20000.99m,
            Qtde = 0,
            Destaque = true,
            Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/macbook-air-m2-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800",
            },
            new() {
            Id = 3,
            CategoriaId = 3,
            Nome = "Apple Watch Series 9",
            Descricao = "O Apple Watch Series 9 é o smartwatch mais avançado da Apple, oferecendo uma combinação perfeita de estilo e funcionalidade. Com um design elegante e uma tela Retina sempre ativa, o Series 9 é ideal para monitorar sua saúde e fitness. Ele possui recursos como monitoramento de frequência cardíaca, rastreamento de atividades físicas, detecção de quedas e suporte a aplicativos de terceiros. O Apple Watch Series 9 é resistente à água e oferece uma bateria de longa duração para mantê-lo conectado durante todo o dia.",
            ValorCusto = 300m,
            ValorVenda = 2500.00m,
            Qtde = 0,
            Destaque = true,
            Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/apple-watch-series-9-gps-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800",
            },
            new() {
            Id = 4,
            CategoriaId = 4,
            Nome = "AirPods Pro",
            Descricao = "Os AirPods Pro são os fones de ouvido sem fio premium da Apple, oferecendo uma experiência de áudio imersiva e cancelamento ativo de ruído. Com um design compacto e confortável, os AirPods Pro se encaixam perfeitamente nos ouvidos. Eles possuem um modo de transparência que permite ouvir o ambiente ao seu redor quando necessário. Os AirPods Pro são resistentes à água e suor, tornando-os ideais para atividades físicas. Com uma bateria de longa duração e suporte a carregamento sem fio, os AirPods Pro são a escolha perfeita para quem busca qualidade de áudio e conveniência.",
            ValorCusto = 700m,
            ValorVenda = 5600.00m,
            Qtde = 0,
            Destaque = true,
            Foto = "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MWP22?wid=940&hei=1112&fmt=png-alpha&.v=1700792800",
            }

        ];
        
            builder.Entity<Produto>().HasData(produtos);
    }
}
