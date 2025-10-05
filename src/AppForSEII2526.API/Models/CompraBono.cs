// Models/CompraBono.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace AppForSEII2526.Models 
{
   
    public class CompraBono : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Range(1, int.MaxValue, ErrorMessage = "CompraBonoId must be > 0")]
        [Display(Name = "Id Compra Bono")]
        public int CompraBonoId { get; set; }

        [Required, StringLength(120, MinimumLength = 2)]
        [Display(Name = "Nombre Cliente")]
        public string NombreCliente { get; set; } = string.Empty;

        [Required, StringLength(120, MinimumLength = 2)]
        [Display(Name = "Apellido 1")]
        public string ApellidoBono1 { get; set; } = string.Empty;

        [StringLength(120)]
        [Display(Name = "Apellido 2")]
        public string? ApellidoBono2 { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha Compra")]
        public DateTime FechaCompraBono { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Metodo de Pago")]
        public MetodoPago MetodoPago { get; set; }

        [Display(Name = "Numero de Bonos")]
        [Range(1, int.MaxValue, ErrorMessage = "nBonos must be >= 1")]
        public int NBonos { get; set; }

        [Display(Name = "Precio Total Bono")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335",
               ErrorMessage = "PrecioTotalBono must be >= 0")]
        public decimal PrecioTotalBono { get; set; }

        
        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (NBonos < 1)
                yield return new ValidationResult("nBonos must be >= 1", new[] { nameof(NBonos) });

            if (PrecioTotalBono < 0)
                yield return new ValidationResult("PrecioTotalBono must be >= 0", new[] { nameof(PrecioTotalBono) });
        }

       
        public enum MetodoPago
        {
            Tarjeta = 0,
            Paypal = 1,
            GooglePay = 2
        }
    }
}
