using ConectaStore.API.Data;
using ConectaStore.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConectaStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly AppDbContext _context;

    public VendasController(AppDbContext context)
    {
        _context = context;
    }

    public class CriarVendaRequest
    {
        public string Cliente { get; set; }
        public List<ItemVendaRequest> Itens { get; set; }
    }

    public class ItemVendaRequest
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> GetVendas()
    {
        var vendas = await _context.Vendas
            .Include(v => v.Itens)
            .ThenInclude(i => i.Produto)
            .OrderByDescending(v => v.Data)
            .ToListAsync();
        return Ok(vendas);
    }

    [HttpPost]
    public async Task<IActionResult> CriarVenda([FromBody] CriarVendaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Cliente))
            return BadRequest(new { erro = "Nome do cliente é obrigatório." });

        if (request.Itens == null || request.Itens.Count == 0)
            return BadRequest(new { erro = "Carrinho vazio." });

        var produtos = await _context.Produtos
            .Where(p => request.Itens.Select(i => i.ProdutoId).Contains(p.Id))
            .ToListAsync();

        var errosEstoque = new List<string>();
        foreach (var itemReq in request.Itens)
        {
            var produto = produtos.FirstOrDefault(p => p.Id == itemReq.ProdutoId);
            if (produto == null)
            {
                errosEstoque.Add($"Produto ID {itemReq.ProdutoId} não encontrado.");
            }
            else if (produto.Qtde < itemReq.Quantidade)
            {
                errosEstoque.Add($"Estoque insuficiente para '{produto.Nome}'. Disponível: {produto.Qtde}, solicitado: {itemReq.Quantidade}.");
            }
        }

        if (errosEstoque.Any())
            return BadRequest(new { erro = "Erro no estoque.", detalhes = errosEstoque });

        var venda = new Venda
        {
            Data = DateTime.Now,
            Cliente = request.Cliente,
            Status = "Concluída",
            Total = 0
        };

        foreach (var itemReq in request.Itens)
        {
            var produto = produtos.First(p => p.Id == itemReq.ProdutoId);
            var itemVenda = new ItemVenda
            {
                ProdutoId = produto.Id,
                Quantidade = itemReq.Quantidade,
                PrecoUnitario = produto.ValorVenda
            };
            venda.Itens.Add(itemVenda);
            venda.Total += produto.ValorVenda * itemReq.Quantidade;
            produto.Qtde -= itemReq.Quantidade;
        }

        _context.Vendas.Add(venda);
        await _context.SaveChangesAsync();

        return Ok(venda);
    }
}
