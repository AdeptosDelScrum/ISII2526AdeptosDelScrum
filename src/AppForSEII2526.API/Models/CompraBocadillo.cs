using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(Bocadillo),
    nameof(Compra))]
    public class CompraBocadillo
    {

        public CompraBocadillo()
        {

        }

        [Required]
        public int Cantidad { get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String NombreBocadillo { get; set; }

        [Required, DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public float Precio {  get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String TipoPan {  get; set; }
        
        
        [Required]
        public Bocadillo Bocadillo {  get; set; }
         
        [Required]
        public Compra Compra {  get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadillo bocadillo &&
                   Cantidad == bocadillo.Cantidad &&
                   NombreBocadillo == bocadillo.NombreBocadillo &&
                   Precio == bocadillo.Precio &&
                   TipoPan == bocadillo.TipoPan &&
                   EqualityComparer<Bocadillo>.Default.Equals(Bocadillo, bocadillo.Bocadillo) &&
                   EqualityComparer<Compra>.Default.Equals(Compra, bocadillo.Compra);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Cantidad, NombreBocadillo, Precio, TipoPan, Bocadillo, Compra);
        }
    }
}
