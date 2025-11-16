
namespace AppForSEII2526.API.Models
{
    public class TipoPan
    {
        public TipoPan() { }
        public TipoPan(int id, string nombre)
        {
            PanId = id;
            Nombre = nombre;
        }

        [Key]
        public int PanId { get; set; }

        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String Nombre { get; set; }

        
         
        
        public List<Bocadillo> Bocadillos { get; set; }


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
