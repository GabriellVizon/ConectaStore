using ConectaStore.API.Data;
using ConectaStore.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ConectaStore.API.Controllers;

public interface ICategoriasController
{
    IActionResult GetCategorias();
}

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase, ICategoriasController
{
    private readonly AppDbContext _context;

    public CategoriasController(AppDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    public IActionResult GetCategorias()
    {
       return Ok(_context.Categorias.ToList());
    }

    [HttpGet("{id}")]
    public ActionResult <Categoria> GetCategoria(int id)
    {
        var categoria = _context.Categorias.Find(id);
        if (categoria == null) 
            return NotFound();
        return Ok(categoria);
    }

    [HttpPost]
    public ActionResult<Categoria> PostCategoria([FromBody] Categoria categoria)
    {
        if (!ModelState.IsValid)
            return BadRequest("Confira os dados enviados");

        _context.Categorias.Add(categoria);
        _context.SaveChanges();
        return CreatedAtAction("GetCategoria", new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id}")]
    public ActionResult PutCategoria(int id, [FromBody] Categoria categoria)
    {
        if (!ModelState.IsValid || id != categoria.Id)
            return BadRequest("Confira os dados enviados");
        
        var oldCategoria = _context.Categorias.Find(id);
        if (oldCategoria == null)
            return NotFound("Categoria não Existe");

        oldCategoria.Nome = categoria.Nome;
        if (categoria.Cor != null)
            oldCategoria.Cor = categoria.Cor;
        if (categoria.Foto != null)
            oldCategoria.Foto = categoria.Foto;

        _context.Entry(oldCategoria).State = EntityState.Modified;
        _context.SaveChanges();
        return NoContent();
    }
        [HttpDelete("{id}")]

        public ActionResult DeleteCategoria(int id)
        {
            var categoria = _context.Categorias.Find(id);
            if (categoria == null)
                return NotFound("Categoria não Existe");

            if (_context.Produtos.Any(p => p.CategoriaId == id))
                return BadRequest("Não é possível excluir a categoria, existem produtos associados a ela.");
            _context.Categorias.Remove(categoria);
            _context.SaveChanges();
            return NoContent();
        }
    }
