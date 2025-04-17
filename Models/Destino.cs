namespace Anda.ServiciosAPI.Models
{
    public class Destino
    {
        public int ID { get; set; }
        public string Siglas { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int ID_Pais { get; set; }
    }
}