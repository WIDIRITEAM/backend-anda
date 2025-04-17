namespace Anda.ServiciosAPI.Models
{
    public class Proveedor
    {
        public int ID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Notas { get; set; }
        public string Moneda { get; set; } = "ARS";
        public bool Activo { get; set; } = true;

        // Nuevo campo para asociar el proveedor a un destino
        public int ID_Destino { get; set; }
    }
}
