using Microsoft.EntityFrameworkCore;
using Anda.ServiciosAPI.Models;

namespace Anda.ServiciosAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Pais> Paises => Set<Pais>();
        public DbSet<Destino> Destinos => Set<Destino>();
        public DbSet<Proveedor> Proveedores => Set<Proveedor>();
        public DbSet<Servicio> Servicios => Set<Servicio>();
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Cotizacion> Cotizaciones => Set<Cotizacion>();
        public DbSet<DetalleCotizacion> DetalleCotizaciones => Set<DetalleCotizacion>();

    }
}