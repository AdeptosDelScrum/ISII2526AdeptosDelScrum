// Models/CompraBono.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // ✅ Solución definitiva: DataType totalmente cualificado
        [System.ComponentModel.DataAnnotations.DataType(
            System.ComponentModel.DataAnnotations.DataType.DateTime)]
        [Display(Name = "Fecha Compra")]
        public DateTime FechaCompraBono { get; set; } = System.DateTime.UtcNow;

        // --------- RELACIÓN con MetodoPago ---------
        [Display(Name = "Metodo de Pago")]
        [Range(1, int.MaxValue, ErrorMessage = "MetodoPagoId must be >= 1")]
        public int MetodoPagoId { get; set; }                 // FK -> MetodoPago (entidad)
        public MetodoPago MetodoPago { get; set; } = null!;   // N:1

        [Display(Name = "Numero de Bonos")]
        [Range(1, int.MaxValue, ErrorMessage = "nBonos must be >= 1")]
        public int NBonos { get; set; }

        [Display(Name = "Precio Total Bono")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335",
               ErrorMessage = "PrecioTotalBono must be >= 0")]
        public decimal PrecioTotalBono { get; set; }

        // --------- RELACIÓN con BonosComprados (1:N) ---------
        public ICollection<BonosComprados> BonosComprados { get; set; } = new List<BonosComprados>();

        // --------- Validaciones ---------
        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (NBonos < 1)
                yield return new ValidationResult("nBonos must be >= 1", new[] { nameof(NBonos) });

            if (PrecioTotalBono < 0)
                yield return new ValidationResult("PrecioTotalBono must be >= 0", new[] { nameof(PrecioTotalBono) });
        }
    }
}
