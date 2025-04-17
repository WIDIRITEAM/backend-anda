using System.ComponentModel.DataAnnotations.Schema;

public class DetalleCotizacion
{
    public int ID { get; set; }

    public int CotizacionID { get; set; }
    public int ID_Servicio { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public int Dia { get; set; }
    public int Orden { get; set; }

}
