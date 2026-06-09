using ConectaStore.API.Data;
using Microsoft.AspNetCore.Mvc;

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
    }
