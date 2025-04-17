using Microsoft.AspNetCore.Mvc;
using Anda.ServiciosAPI.Data;
using Anda.ServiciosAPI.Models;
using System.Text.RegularExpressions;

namespace Anda.ServiciosAPI.Controllers
{
    [ApiController]
    [Route("api/servicios")]
    public class ServiciosFiltradosController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ServiciosFiltradosController(AppDbContext context) => _context = context;

        public class MensajeUsuario
        {
            public string Mensaje { get; set; } = string.Empty;
        }

        [HttpPost("filtrar-mensaje")]
        public IActionResult FiltrarDesdeMensaje([FromBody] MensajeUsuario input)
        {
            var mensaje = input.Mensaje.ToLower();

            string destino = ExtraerDestino(mensaje);
            int dias = ExtraerDias(mensaje);
            decimal presupuesto = ExtraerPresupuesto(mensaje);

            if (destino == "") return BadRequest("No se pudo identificar el destino.");

            var destinoId = _context.Destinos.FirstOrDefault(d => d.Siglas.ToLower().Contains(destino.ToLower()) || d.Descripcion.ToLower().Contains(destino.ToLower()))?.ID;
            if (destinoId == null) return NotFound("Destino no encontrado en la base de datos.");

            var servicios = _context.Servicios
                .Where(s => s.ID_Destino == destinoId && s.DuracionDias <= dias && s.Activo)
                .OrderBy(s => s.PrecioUnitario)
                .ToList();

            var resultado = new List<Servicio>();
            decimal acumulado = 0;

            foreach (var servicio in servicios)
            {
                if (acumulado + servicio.PrecioUnitario <= presupuesto * 1000) // asume dólares a pesos
                {
                    resultado.Add(servicio);
                    acumulado += servicio.PrecioUnitario;
                }
            }

            return Ok(new
            {
                destino_detectado = destino,
                dias,
                presupuesto_usd = presupuesto,
                total_estimado = acumulado,
                servicios = resultado
            });
        }

        private string ExtraerDestino(string mensaje)
        {
            var destinos = _context.Destinos.Select(d => d.Descripcion.ToLower()).ToList();
            return destinos.FirstOrDefault(mensaje.Contains) ?? "";
        }

        private int ExtraerDias(string mensaje)
        {
            var match = Regex.Match(mensaje, @"(\d+)\s*d[ií]as?");
            return match.Success ? int.Parse(match.Groups[1].Value) : 3;
        }

        private decimal ExtraerPresupuesto(string mensaje)
        {
            var match = Regex.Match(mensaje, @"(\d+[.,]?\d*)\s*(usd|d[oó]lares?)");
            return match.Success ? decimal.Parse(match.Groups[1].Value.Replace(",", ".")) : 500;
        }
    }
}