using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.DTOs
{
    public class BonoBocadilloDTO
    {
        public long BonoId { get; set; }

        [Required]
        [StringLength(40, ErrorMessage = "El nombre no puede tener mas de 40 caracteres.")]
        [RegularExpression(@"^[A-Z][a-zA-Z0-9 '\-]*$", ErrorMessage = "El nombre debe empezar con mayuscula.")]
        public string Nombre { get; set; } = "";

        [Display(Name = "Numero de bocadillos")]
        [Range(1, int.MaxValue, ErrorMessage = "nBocadillos debe ser al menos 1.")]
        public int NBocadillos { get; set; }

        [Display(Name = "Stock")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad disponible no puede ser negativa.")]
        public int CantidadDisponible { get; set; }

        [Display(Name = "PVP")]
        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "0", "79228162514264337593543950335",
            ErrorMessage = "PVP debe ser mayor o igual que 0.")]
        public decimal Pvp { get; set; }

        // Datos del tipo asociado (segun el diagrama: TipoBocadillo)
        [Display(Name = "Id tipo")]
        [Required]
        public long IdTipo { get; set; }

        [Display(Name = "Tipo (vegano|vegetariano|sin gluten|normal)")]
        [StringLength(30)]
        public string? NombreTipo { get; set; }
    }
}
