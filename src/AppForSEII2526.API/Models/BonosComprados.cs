// Models/BonosComprados.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace AppForSEII2526.Models
{
    public class BonosComprados : IValidatableObject
    {
        [Display(Name = "Bono Id")]
        [Range(1, int.MaxValue, ErrorMessage = "BonoId must be >= 1")]
        public int BonoId { get; set; }   // FK -> BonoBocadillo

        [Display(Name = "Compra Id")]
        [Range(1, int.MaxValue, ErrorMessage = "CompraId must be >= 1")]
        public int CompraId { get; set; } // FK -> CompraBono

        [Display(Name = "Cantidad")]
        [Range(1, int.MaxValue, ErrorMessage = "Cantidad must be >= 1")]
        public int Cantidad { get; set; }

        [Display(Name = "Precio Bono")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "PrecioBono must be >= 0")]
        public decimal PrecioBono { get; set; }

        // --------- RELACIONES ---------
        public BonoBocadillo Bono { get; set; } = null!;   // N:1
        public CompraBono Compra { get; set; } = null!;    // N:1

        // --------- Validaciones ---------
        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (Cantidad <= 0)
                yield return new ValidationResult("Cantidad must be >= 1", new[] { nameof(Cantidad) });
            if (PrecioBono < 0)
                yield return new ValidationResult("PrecioBono must be >= 0", new[] { nameof(PrecioBono) });
        }
    }
}

