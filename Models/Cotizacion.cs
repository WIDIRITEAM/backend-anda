public class Cotizacion
{
    public int ID { get; set; }
    public int ID_Cliente { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public decimal Total { get; set; }

    public List<DetalleCotizacion> Detalles { get; set; } = new();
}
