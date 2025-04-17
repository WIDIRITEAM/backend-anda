using Microsoft.AspNetCore.Mvc;
using Anda.ServiciosAPI.Data;
using Anda.ServiciosAPI.Models;

namespace Anda.ServiciosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context) => _context = context;

        // Obtener todos los proveedores
        [HttpGet]
        public IActionResult Get() => Ok(_context.Proveedores.ToList());

        // Obtener un proveedor por ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var proveedor = _context.Proveedores.Find(id);
            return proveedor is null ? NotFound() : Ok(proveedor);
        }

        // Crear proveedor nuevo
        [HttpPost]
        public IActionResult Post(Proveedor proveedor)
        {
            _context.Proveedores.Add(proveedor);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = proveedor.ID }, proveedor);
        }

        // Actualizar proveedor
        [HttpPut("{id}")]
        public IActionResult Put(int id, Proveedor proveedorActualizado)
        {
            var proveedor = _context.Proveedores.Find(id);
            if (proveedor is null) return NotFound();

            proveedor.Nombre = proveedorActualizado.Nombre;
            proveedor.Notas = proveedorActualizado.Notas;
            proveedor.Moneda = proveedorActualizado.Moneda;
            proveedor.Activo = proveedorActualizado.Activo;

            _context.SaveChanges();
            return NoContent();
        }

        // Eliminar proveedor
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var proveedor = _context.Proveedores.Find(id);
            if (proveedor is null) return NotFound();

            _context.Proveedores.Remove(proveedor);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
