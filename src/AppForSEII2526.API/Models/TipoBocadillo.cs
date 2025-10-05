// /Models/TipoBocadillo.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace AppForSEII2526.Models  // <-- cambia a tu namespace real
{
    public class TipoBocadillo
    {
        // Primary key (no identity; controlled by app)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Range(1, int.MaxValue, ErrorMessage = "IdTipo must be > 0")]
        [Display(Name = "Id Tipo")]
        public int IdTipo { get; set; }

        // Required + max length + display name
        [Required(ErrorMessage = "NombreTipo is required")]
        [StringLength(100, ErrorMessage = "Max length is 100")]
        [Display(Name = "Nombre del Tipo")]
        public string NombreTipo { get; set; } = string.Empty;

        // First pass: no navigation properties, no FKs
        public ICollection<BonoBocadillo> Bonos { get; set; } = new List<BonoBocadillo>();
    }
}
