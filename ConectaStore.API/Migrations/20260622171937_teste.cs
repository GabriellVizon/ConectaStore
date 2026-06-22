using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConectaStore.API.Migrations
{
    /// <inheritdoc />
    public partial class teste : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Cor = table.Column<string>(type: "nvarchar(26)", maxLength: 26, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cliente = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoriaId = table.Column<int>(type: "int", nullable: false),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Qtde = table.Column<int>(type: "int", nullable: false),
                    ValorCusto = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    ValorVenda = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    Destaque = table.Column<bool>(type: "bit", nullable: false),
                    Foto = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Produtos_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItensVenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VendaId = table.Column<int>(type: "int", nullable: false),
                    ProdutoId = table.Column<int>(type: "int", nullable: false),
                    Quantidade = table.Column<int>(type: "int", nullable: false),
                    PrecoUnitario = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItensVenda", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Produtos_ProdutoId",
                        column: x => x.ProdutoId,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItensVenda_Vendas_VendaId",
                        column: x => x.VendaId,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categorias",
                columns: new[] { "Id", "Cor", "Foto", "Nome" },
                values: new object[,]
                {
                    { 1, "#FF5733", null, "Smartphones" },
                    { 2, "#33FF57", null, "Notebooks" },
                    { 3, "#3357FF", null, "SmartWatches" },
                    { 4, "#FF33A1", null, "Fones de Ouvido" },
                    { 5, "#A855F7", null, "Tablets" },
                    { 6, "#F59E0B", null, "Acessórios" },
                    { 7, "#10B981", null, "Gaming" },
                    { 8, "#EC4899", null, "Áudio" }
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "CategoriaId", "Descricao", "Destaque", "Foto", "Nome", "Qtde", "ValorCusto", "ValorVenda" },
                values: new object[,]
                {
                    { 1, 1, "O iPhone 17 Pro é o mais recente lançamento da Apple, trazendo um design elegante e recursos avançados. Com uma tela Super Retina XDR de 6,1 polegadas, o dispositivo oferece uma experiência visual imersiva. Equipado com o chip A17 Bionic, o iPhone 17 Pro proporciona desempenho excepcional e eficiência energética. O sistema de câmera tripla inclui uma lente principal de 12 MP, uma lente ultra-angular de 12 MP e uma lente telefoto de 12 MP, permitindo fotos e vídeos de alta qualidade.", true, "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/iphone-17-pro-gold-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800", "iPhone 17 Pro", 5, 1000m, 10000.50m },
                    { 2, 2, "O MacBook Air é o notebook mais leve e portátil da Apple. Com o chip M2, oferece desempenho excepcional e eficiência energética. A tela Retina de 13,6 polegadas proporciona uma experiência visual imersiva. Até 18 horas de autonomia.", true, "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/macbook-air-m2-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800", "MacBook Air M2", 3, 2000m, 20000.99m },
                    { 3, 3, "Smartwatch mais avançado da Apple. Tela Retina sempre ativa, monitoramento de frequência cardíaca, rastreamento de atividades, detecção de quedas. Resistente à água.", true, "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/apple-watch-series-9-gps-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800", "Apple Watch Series 9", 8, 300m, 2500.00m },
                    { 4, 4, "Fones de ouvido sem fio premium com cancelamento ativo de ruído, modo transparência, resistentes à água e suor. Carregamento sem fio incluso.", true, "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/MWP22?wid=940&hei=1112&fmt=png-alpha&.v=1700792800", "AirPods Pro 2", 12, 700m, 5600.00m },
                    { 5, 5, "Tablet premium com tela Dynamic AMOLED 2X de 11 polegadas, processador Snapdragon 8 Gen 2, caneta S-PEN inclusa, resistência à água IP68.", true, "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-tab-s9/bu_tab_s9_lunar_silver.jpg", "Samsung Galaxy Tab S9", 6, 1800m, 8500.00m },
                    { 6, 5, "iPad Air com chip M2, tela Liquid Retina de 11 polegadas, compatível com Apple Pencil e Magic Keyboard. Ideal para produtividade e criatividade.", false, "https://store.storeimages.cdn-apple.com/4982/as-images.apple.com/is/ipad-air-m2-select?wid=940&hei=1112&fmt=png-alpha&.v=1700792800", "iPad Air M2", 4, 2200m, 9200.00m },
                    { 7, 6, "Carregador USB-C PD 65W compacto, compatível com notebooks e smartphones. Carregamento rápido GaN, proteção contra sobrecarga.", false, "https://m.media-amazon.com/images/I/51L3tG7tJTL._AC_SX679_.jpg", "Carregador Turbo 65W", 25, 45m, 189.90m },
                    { 8, 6, "Capa protetora universal para notebooks de até 15.6 polegadas. Material resistente a impactos, interior acolchoado, alça de transporte.", false, "https://m.media-amazon.com/images/I/71b8J1fVqOL._AC_SX679_.jpg", "Capa para Notebook 15\"", 18, 35m, 149.90m },
                    { 9, 7, "Mouse gamer com sensor óptico de 16000 DPI, 8 botões programáveis, iluminação RGB personalizável, cabo trançado USB.", true, "https://m.media-amazon.com/images/I/61Hn4TCjtDL._AC_SX679_.jpg", "Mouse Gamer RGB", 15, 60m, 249.90m },
                    { 10, 7, "Teclado mecânico RGB com switches azuis, estrutura em alumínio, teclas PBT double-shot, anti-ghosting, USB-C destacável.", false, "https://m.media-amazon.com/images/I/71b8J1fVqOL._AC_SX679_.jpg", "Teclado Mecânico Gamer", 10, 110m, 449.90m },
                    { 11, 7, "Headset gamer com som surround 7.1 virtual, drivers de 50mm, microfone com cancelamento de ruído, almofadas em couro respirável.", true, "https://m.media-amazon.com/images/I/61CGHv6kmWL._AC_SX679_.jpg", "Headset Gamer 7.1", 7, 90m, 379.90m },
                    { 12, 8, "Caixa de som portátil Bluetooth 5.3, 30W RMS, graves potentes, resistência à água IPX7, bateria de 12 horas.", true, "https://m.media-amazon.com/images/I/71b8J1fVqOL._AC_SX679_.jpg", "Caixa de Som Bluetooth", 14, 80m, 329.90m },
                    { 13, 8, "Soundbar 2.1 canais com subwoofer wireless, 200W RMS, Bluetooth 5.0, entradas HDMI ARC e óptica, controle remoto incluso.", false, "https://m.media-amazon.com/images/I/61CGHv6kmWL._AC_SX679_.jpg", "Soundbar 2.1", 5, 200m, 899.90m },
                    { 14, 1, "Smartphone premium Samsung com tela Dynamic AMOLED 2X de 6.8 polegadas, processador Snapdragon 8 Gen 3, câmera de 200MP, S-PEN integrada.", true, "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-s24-ultra/bu_s24_ultra_titanium_gray.jpg", "Galaxy S24 Ultra", 7, 2500m, 11000.00m },
                    { 15, 2, "Notebook premium Samsung com tela AMOLED 3K de 16 polegadas, processador Intel Core Ultra 7, 16GB RAM, SSD 512GB.", true, "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-book4-pro/bu_book4_pro_platinum_silver.jpg", "Galaxy Book4 Pro", 2, 3000m, 15000.00m },
                    { 16, 4, "Fones de ouvido True Wireless com cancelamento ativo de ruído adaptativo, som 24-bit Hi-Fi, bateria de 6h + 18h estojo.", false, "https://images.samsung.com/is/image/samsung/p6p-br/galaxy-buds3-pro/bu_buds3_pro_silver.jpg", "Galaxy Buds3 Pro", 10, 500m, 2200.00m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_ProdutoId",
                table: "ItensVenda",
                column: "ProdutoId");

            migrationBuilder.CreateIndex(
                name: "IX_ItensVenda_VendaId",
                table: "ItensVenda",
                column: "VendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItensVenda");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Vendas");

            migrationBuilder.DropTable(
                name: "Categorias");
        }
    }
}
