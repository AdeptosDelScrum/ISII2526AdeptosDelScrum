using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.Models
{
    public class Resenya
    {
        [Key]
        public int Id { get; set; }

        [Required,StringLength(150,ErrorMessage = "La descripción puede tener como máximo 150 caracteres", MinimumLength = 10)]
        [Display(Name = "Descripción")]
        public string descripcion { get; set; }

        [Required]
        [DataType(DataType.Date), Display(Name = "Fecha de publicación")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime fechaPublicacion { get; set; }

        [Required,StringLength(20, ErrorMessage = "El nombre de usuario debe tener máximo 20 caracteres")]
        [Display(Name = "Nombre de usuario")]
        public string? nombreUsuario {  get; set; }

        [Required,StringLength(25, ErrorMessage = "El título debe tener máximo 25 caracteres", MinimumLength = 1)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        [Display(Name = "Título")]
        public string titulo {  get; set; }

        [Required]
        [Display(Name = "Valoración general")]
        public rate valoracion { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Resenya resenya &&
                   id == resenya.id &&
                   descripcion == resenya.descripcion &&
                   fechaPublicacion == resenya.fechaPublicacion &&
                   nombreUsuario == resenya.nombreUsuario &&
                   titulo == resenya.titulo;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(id, descripcion, fechaPublicacion, nombreUsuario, titulo);
        }
    }
}
