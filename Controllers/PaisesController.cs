using Microsoft.AspNetCore.Mvc;
using Anda.ServiciosAPI.Data;
using Anda.ServiciosAPI.Models;

namespace Anda.ServiciosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaisesController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PaisesController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Get() => Ok(_context.Paises.ToList());

        [HttpPost]
        public IActionResult Post(Pais pais)
        {
            _context.Paises.Add(pais);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = pais.ID }, pais);
        }
    }
}