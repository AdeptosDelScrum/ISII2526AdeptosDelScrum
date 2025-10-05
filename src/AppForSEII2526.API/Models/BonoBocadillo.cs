// Models/BonoBocadillo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace AppForSEII2526.Models  // <-- cambia al namespace real
{
   
     Entidad BonoBocadillo (primera pasada: sin relaciones).
   
    public class BonoBocadillo : IValidatableObject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Range(1, int.MaxValue, ErrorMessage = "BonoId must be > 0")]
        [Display(Name = "Id Bono")]
        public int BonoId { get; set; }

        [Required(ErrorMessage = "Nombre is required")]
        [StringLength(120, MinimumLength = 2, ErrorMessage = "Length 2..120")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Display(Name = "N Bocadillos")]
        [Range(1, int.MaxValue, ErrorMessage = "NBocadillos must be >= 1")]
        public int NBocadillos { get; set; }

        [Display(Name = "Cantidad Disponible")]
        [Range(0, int.MaxValue, ErrorMessage = "CantidadDisponible must be >= 0")]
        public int CantidadDisponible { get; set; }

        [Display(Name = "PVP")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "PVP must be >= 0")]
        public decimal PVP { get; set; }

       
        public IEnumerable<ValidationResult> Validate(ValidationContext ctx)
        {
            if (CantidadDisponible > NBocadillos)
                yield return new ValidationResult(
                    "CantidadDisponible must be <= NBocadillos",
                    new[] { nameof(CantidadDisponible) });
        }

        public override string ToString() => $"{BonoId}:{Nombre} ({CantidadDisponible}/{NBocadillos})";
    }
}
