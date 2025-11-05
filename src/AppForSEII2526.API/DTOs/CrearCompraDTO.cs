using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AppForSEII2526.API.DTOs
{
    /// <summary>
    /// Peticion de compra de bonos de bocadillo.
    /// El backend calculara precios y totales desde la BD.
    /// </summary>
    public class CrearCompraDTO
    {
        [Required]
        [StringLength(80)]
        public string NombreCompleto { get; set; } = "";

        [Required]
        [StringLength(120)]
        public string Apellidos { get; set; } = "";

        /// <summary>
        /// Identificador del metodo de pago existente (tabla/clase de metodos de pago).
        /// </summary>
        [Required]
        public long MetodoPagoId { get; set; }

        /// <summary>
        /// Bonos seleccionados con la cantidad deseada.
        /// </summary>
        [Required]
        [MinLength(1, ErrorMessage = "Debe incluir al menos un bono.")]
        public List<CrearCompraItemDTO> Items { get; set; } = new();
    }

    public class CrearCompraItemDTO
    {
        [Required]
        public long BonoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }
    }
}
