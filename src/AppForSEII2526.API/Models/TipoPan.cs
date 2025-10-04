
namespace AppForSEII2526.API.Models
{
    public class TipoPan
    {
        public TipoPan() { }

        [Key]
        public int PanId { get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String Nombre { get; set; }

        
         
        [Required]
        public List<Bocadillo> Bocadillos { get; set; }

         */

        public override bool Equals(object? obj)
        {
            return obj is TipoPan pan &&
                   PanId == pan.PanId &&
                   Nombre == pan.Nombre;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PanId, Nombre);
        }
    }
}
