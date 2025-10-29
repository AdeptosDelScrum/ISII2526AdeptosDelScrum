using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs
{
    /// <summary>
    /// Peticion de compra de bonos de bocadillo.
    /// El backend calculara precios y totales a partir de la BD.
    /// </summary>
    public class CrearCompraDTO
    {
        [Required]
        [StringLength(80)]
        public string NombreCompleto { get; set; } = "";

        [Required]
        [StringLength(120)]
        public string Apellidos { get; set; } = "";

        [Required]
        public MetodoPago MetodoPago { get; set; }

        /// <summary>
        /// Bonos seleccionados con la cantidad deseada.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un bono.")]
        public List<CrearCompraItemDTO> Items { get; set; } = new();
    }

    /// <summary>
    /// Linea de compra: bono + cantidad.
    /// </summary>
    public class CrearCompraItemDTO
    {
        [Required]
        public long BonoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }
    }

    /// <summary>
    /// Enum de metodos de pago (ajusta a tu dominio si usas otros).
    /// </summary>
    public enum MetodoPago
    {
        Tarjeta = 0,
        Paypal = 1,
        GooglePay = 2
        // Efectivo = 3, Bizum = 4, etc. si los necesitas
    }
}
