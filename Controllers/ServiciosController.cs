using Microsoft.AspNetCore.Mvc;
using Anda.ServiciosAPI.Data;
using Anda.ServiciosAPI.Models;

namespace Anda.ServiciosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServiciosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ServiciosController(AppDbContext context) => _context = context;

        // Obtener todos los servicios
        [HttpGet]
        public IActionResult Get() => Ok(_context.Servicios.ToList());

        // Obtener un servicio por ID
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var servicio = _context.Servicios.Find(id);
            return servicio is null ? NotFound() : Ok(servicio);
        }

        // Crear un nuevo servicio
        [HttpPost]
        public IActionResult Post(Servicio servicio)
        {
            _context.Servicios.Add(servicio);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = servicio.ID }, servicio);
        }

        // Actualizar un servicio existente
        [HttpPut("{id}")]
        public IActionResult Put(int id, Servicio actualizado)
        {
            var servicio = _context.Servicios.Find(id);
            if (servicio is null) return NotFound();

            servicio.ID_Proveedor = actualizado.ID_Proveedor;
            servicio.ID_Destino = actualizado.ID_Destino;
            servicio.TipoServicio = actualizado.TipoServicio;
            servicio.Descripcion = actualizado.Descripcion;
            servicio.PrecioUnitario = actualizado.PrecioUnitario;
            servicio.Modalidad = actualizado.Modalidad;
            servicio.DuracionDias = actualizado.DuracionDias;
            servicio.Activo = actualizado.Activo;

            _context.SaveChanges();
            return NoContent();
        }

        // Eliminar un servicio
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var servicio = _context.Servicios.Find(id);
            if (servicio is null) return NotFound();

            _context.Servicios.Remove(servicio);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
