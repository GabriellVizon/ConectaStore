using ConectaStore.API.Data;
using ConectaStore.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConectaStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Produto>> GetProdutos()
    {
        var produtos = _context.Produtos
        .Include(p => p.Categoria)
        .ToList();
        return Ok(produtos);
    }

    // GET api/produtos/1
    [HttpGet("{id}")]
    public ActionResult<Produto> GetProduto(int id)
    {
        var produto = _context.Produtos
        .Where(p => p.Id == id)
        .Include(p => p.Categoria)
        .FirstOrDefault();

        if (produto == null) return NotFound("Produto não encontrado.");

        return Ok(produto);
    }

    // GET: api/produtos/categoria/1
    [HttpGet("categoria/{categoriaId}")]
    public ActionResult<IEnumerable<Produto>> GetProdutosPorCategoria(int categoriaId)
    {
        var produtos = _context.Produtos
        .Where(p => p.CategoriaId == categoriaId)
        .Include(p => p.Categoria)
        .ToList();
        return Ok(produtos);
    }

    [HttpGet("destaques")]
    public ActionResult<IEnumerable<Produto>> GetProdutosDestaques()
    {
        var produtos = _context.Produtos
        .Where(p => p.Destaque)
        .Include(p => p.Categoria)
        .ToList();

        if (!produtos.Any()) return NotFound("Nenhum produto em destaque encontrado.");
        return Ok(produtos);
    }

    [HttpPost]
    public ActionResult<Produto> PostProduto([FromBody] Produto produto)
    {
        if (!ModelState.IsValid)
            return BadRequest("Confira os dados enviados");

        if (!_context.Categorias.Any(c => c.Id == produto.CategoriaId))
            return BadRequest("Categoria não existe");

        _context.Produtos.Add(produto);
        _context.SaveChanges();
        return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
    }
    // PUT api/produtos/1
    [HttpPut("{id}")]
    public ActionResult PutProduto(int id, [FromBody] Produto produto)
    {
        if (!ModelState.IsValid || id != produto.Id)
            return BadRequest("Confira os dados enviados");

        var oldProduto = _context.Produtos.Find(id);
        if (oldProduto == null)
            return NotFound("Produto não encontrado");

        if (!CategoriaExiste(produto.CategoriaId))
            return BadRequest("Categoria não existe");

        oldProduto.Nome = produto.Nome;
        if (produto.Descricao != null) oldProduto.Descricao = produto.Descricao;
        oldProduto.Qtde = produto.Qtde;
        oldProduto.ValorCusto = produto.ValorCusto;
        oldProduto.ValorVenda = produto.ValorVenda;
        oldProduto.Destaque = produto.Destaque;
        oldProduto.CategoriaId = produto.CategoriaId;
        if (produto.Foto != null) oldProduto.Foto = produto.Foto;

        _context.Entry(oldProduto).State = EntityState.Modified;
        _context.SaveChanges();
        return NoContent();
    }

    [HttpGet("busca/{texto}")]
    public ActionResult<IEnumerable<Produto>> BuscarProdutos(string texto)
    {
        var produtos = _context.Produtos
            .Where(p => p.Nome.Contains(texto))
            .Include(p => p.Categoria)
            .ToList();

        if (!produtos.Any())
            return NotFound("Nenhum produto encontrado.");

        return Ok(produtos);
    }

    // DELETE api/produtos/1
    [HttpDelete("{id}")]
    public ActionResult DeleteProduto(int id)
    {
        var produto = _context.Produtos.Find(id);
        if (produto == null)
            return NotFound("Produto não encontrado");
        _context.Produtos.Remove(produto);
        _context.SaveChanges();
        return NoContent();
    }

    // ===== Admin endpoints (incluem dados de custo) =====

    public class ProdutoAdminRequest
    {
        public int CategoriaId { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public int Qtde { get; set; }
        public decimal ValorCusto { get; set; }
        public decimal ValorVenda { get; set; }
        public bool Destaque { get; set; }
        public string Foto { get; set; }
    }

    [HttpGet("admin")]
    public async Task<ActionResult> GetProdutosAdmin()
    {
        var produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .OrderBy(p => p.Id)
            .ToListAsync();
        var result = produtos.Select(p => new {
            p.Id, p.CategoriaId,
            CategoriaNome = p.Categoria?.Nome,
            p.Nome, p.Descricao, p.Qtde,
            p.ValorCusto, p.ValorVenda, p.Destaque, p.Foto
        });
        return Ok(result);
    }

    [HttpPost("admin")]
    public async Task<ActionResult> PostProdutoAdmin([FromBody] ProdutoAdminRequest request)
    {
        var produto = new Produto
        {
            CategoriaId = request.CategoriaId,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Qtde = request.Qtde,
            ValorCusto = request.ValorCusto,
            ValorVenda = request.ValorVenda,
            Destaque = request.Destaque,
            Foto = request.Foto
        };
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
    }

    [HttpPut("admin/{id}")]
    public async Task<ActionResult> PutProdutoAdmin(int id, [FromBody] ProdutoAdminRequest request)
    {
        var old = await _context.Produtos.FindAsync(id);
        if (old == null) return NotFound("Produto não encontrado");
        if (!_context.Categorias.Any(c => c.Id == request.CategoriaId))
            return BadRequest("Categoria não existe");
        old.CategoriaId = request.CategoriaId;
        old.Nome = request.Nome;
        if (request.Descricao != null) old.Descricao = request.Descricao;
        old.Qtde = request.Qtde;
        old.ValorCusto = request.ValorCusto;
        old.ValorVenda = request.ValorVenda;
        old.Destaque = request.Destaque;
        if (request.Foto != null) old.Foto = request.Foto;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private bool CategoriaExiste(int id)
    {
        return _context.Categorias.Any(c => c.Id == id);
    }
}
