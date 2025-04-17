namespace Anda.ServiciosAPI.Models
{
    public class Servicio
    {
        public int ID { get; set; }
        public int ID_Proveedor { get; set; }
        public int ID_Destino { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public string Modalidad { get; set; } = "por_persona";
        public int DuracionDias { get; set; }
        public bool Activo { get; set; } = true;
    }
}