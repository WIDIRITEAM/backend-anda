using Microsoft.AspNetCore.Mvc;
using Anda.ServiciosAPI.Data;
using Anda.ServiciosAPI.Models;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Anda.ServiciosAPI.Controllers
{
    [ApiController]
    [Route("api/ia")]
    public class IACotizacionController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ServicioIAClient _iaClient;
        private readonly IConfiguration _config;

        public IACotizacionController(AppDbContext context, ServicioIAClient iaClient, IConfiguration config)
        {
            _context = context;
            _iaClient = iaClient;
            _config = config;
        }

        public class SolicitudCotizacion
        {
            public string Mensaje { get; set; } = string.Empty;
        }
        [HttpPost("guardar")]
        public async Task<IActionResult> GuardarCotizacion([FromBody] Cotizacion cotizacion)
        {
            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();
            return Ok(cotizacion.ID);
        }

        [HttpPost("cotizar")]
        public async Task<IActionResult> CotizarDesdeMensaje([FromBody] SolicitudCotizacion input)
        {
            var usarMock = false;

            Console.WriteLine($"📢 USE_MOCK_IA: {usarMock}"); // <-- Añadí esto

            var mensaje = input.Mensaje.ToLower();

            var destinoDetectado = _context.Destinos
                .FirstOrDefault(d => mensaje.Contains(d.Descripcion.ToLower()));

            if (destinoDetectado == null)
                return BadRequest("No se pudo detectar el destino en el mensaje.");

            var servicios = _context.Servicios
                .Where(s => s.ID_Destino == destinoDetectado.ID && s.Activo)
                .Select(s => new
                {
                    id = s.ID,
                    tipo = s.TipoServicio,
                    descripcion = s.Descripcion,
                    precio = s.PrecioUnitario,
                    dia = s.DuracionDias
                })
                .ToList<object>();

            if (usarMock)
            {
                var mock = new
                {
                    itinerario = new[]
                    {
                        new
                        {
                            dia = 1,
                            servicios = new[]
                            {
                                new { id = 1, descripcion = "Transfer privado al hotel", tipo = "Transfer", precio = 10000 },
                                new { id = 2, descripcion = "Cena romántica frente al lago", tipo = "Cena", precio = 12000 }
                            }
                        },
                        new
                        {
                            dia = 2,
                            servicios = new[]
                            {
                                new { id = 3, descripcion = "Excursión de día completo", tipo = "Tour", precio = 25000 }
                            }
                        },
                        new
                        {
                            dia = 3,
                            servicios = new[]
                            {
                                new { id = 4, descripcion = "Trekking con guía", tipo = "Excursión", precio = 18000 },
                                new { id = 5, descripcion = "Noche en hotel boutique", tipo = "Hotel", precio = 30000 }
                            }
                        }
                    }
                };

                // calcular total desde los precios
                var total = mock.itinerario.SelectMany(d => d.servicios).Sum(s => s.precio);

                var respuestaMock = new
                {
                    itinerario = mock.itinerario,
                    estimado_total = total
                };

                return Ok(respuestaMock);
            }

            // Si no se usa el mock, llamar a la IA real
            var resultado = await _iaClient.EnviarCotizacionAsync(input.Mensaje, servicios);

            if (resultado == null)
                return StatusCode(500, "Error llamando al servicio de IA.");

            return Ok(JsonSerializer.Serialize(resultado, new JsonSerializerOptions { WriteIndented = true }));
        }

        [HttpGet("cotizaciones")]
        public IActionResult ObtenerCotizaciones()
        {
            var cotizaciones = _context.Cotizaciones
                .Include(c => c.Detalles)
                .Select(c => new
                {
                    c.ID,
                    c.ID_Cliente,
                    Cliente = _context.Clientes
                                .Where(cli => cli.ID == c.ID_Cliente)
                                .Select(cli => cli.Nombre + " " + cli.Apellido)
                                .FirstOrDefault(),
                    c.Fecha,
                    c.Total
                })
                .ToList();

            return Ok(cotizaciones);
        }
        [HttpGet("cotizaciones/{id}")]
        public IActionResult ObtenerCotizacionPorId(int id)
        {
            var cotizacion = _context.Cotizaciones
                .Include(c => c.Detalles)
                .FirstOrDefault(c => c.ID == id);

            if (cotizacion == null)
                return NotFound("Cotización no encontrada.");

            var detalles = cotizacion.Detalles.Select(d =>
            {
                var servicio = _context.Servicios.FirstOrDefault(s => s.ID == d.ID_Servicio);
                return new
                {
                    d.ID,
                    d.Dia,
                    d.Orden,
                    d.Cantidad,
                    d.PrecioUnitario,
                    d.ID_Servicio,
                    servicio?.Descripcion,
                    servicio?.ID_Proveedor,
                    servicio?.ID_Destino,
                    servicio?.TipoServicio
                };
            }).ToList();

            return Ok(new
            {
                cotizacion.ID,
                cotizacion.ID_Cliente,
                cotizacion.Fecha,
                cotizacion.Total,
                detalles
            });
        }
        [HttpPut("cotizaciones/{id}")]
        public async Task<IActionResult> ActualizarCotizacion(int id, [FromBody] Cotizacion cotizacion)
        {
            var existente = await _context.Cotizaciones.Include(c => c.Detalles).FirstOrDefaultAsync(c => c.ID == id);
            if (existente == null) return NotFound();

            existente.Total = cotizacion.Total;
            existente.Fecha = cotizacion.Fecha;
            existente.ID_Cliente = cotizacion.ID_Cliente;

            // Eliminar detalles anteriores
            _context.DetalleCotizaciones.RemoveRange(existente.Detalles);

            // Agregar nuevos detalles
            foreach (var detalle in cotizacion.Detalles)
            {
                detalle.CotizacionID = id;
                _context.DetalleCotizaciones.Add(detalle);
            }

            await _context.SaveChangesAsync();
            return Ok(existente);
        }


    }

}
