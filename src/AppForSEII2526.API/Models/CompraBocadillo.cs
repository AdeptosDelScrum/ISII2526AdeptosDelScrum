using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.Models
{
    public class CompraBocadillo
    {

        public CompraBocadillo()
        {

        }

        [Key]
        public int BocadilloId { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public int CompraId {  get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String NombreBocadillo { get; set; }

        [Required, DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public float Precio {  get; set; }

        [Required, StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String Tipopan {  get; set; }
        
        /*
         [Required]
        public Bocadillo Bocadillo {  get; set; }
         
        [Required]
        public Compra Compra {  get; set; }
         */

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadillo bocadillo &&
                   BocadilloId == bocadillo.BocadilloId &&
                   Cantidad == bocadillo.Cantidad &&
                   CompraId == bocadillo.CompraId &&
                   NombreBocadillo == bocadillo.NombreBocadillo &&
                   Precio == bocadillo.Precio;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, Cantidad, CompraId, NombreBocadillo, Precio);
        }
    }
}
