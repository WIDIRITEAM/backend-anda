using Microsoft.AspNetCore.Mvc;
using Anda.ServiciosAPI.Data;
using Anda.ServiciosAPI.Models;

namespace Anda.ServiciosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DestinosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public DestinosController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Get() => Ok(_context.Destinos.ToList());

        [HttpPost]
        public IActionResult Post(Destino destino)
        {
            _context.Destinos.Add(destino);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { id = destino.ID }, destino);
        }
    }
}