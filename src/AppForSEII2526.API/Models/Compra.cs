using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.Models
{
    public class Compra
    {

        public Compra() { }

        [Key]
        public int CompraId { get; set; }

        [Required, StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String Apellido_1Cliente {  get; set; }

        [Required, StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String Apellido_2Cliente { get; set; }

        [Required, DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCompra {  get; set; }

        [Required]
        public int nBocadillos {  get; set; }

        [Required, StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String NombreCliente {  get; set; }

        [Required, DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public float PrecioTotal {  get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Compra compra &&
                   CompraId == compra.CompraId &&
                   Apellido_1Cliente == compra.Apellido_1Cliente &&
                   Apellido_2Cliente == compra.Apellido_2Cliente &&
                   FechaCompra == compra.FechaCompra &&
                   nBocadillos == compra.nBocadillos &&
                   NombreCliente == compra.NombreCliente &&
                   PrecioTotal == compra.PrecioTotal &&
                   EqualityComparer<MetodoPago>.Default.Equals(MetodoPago, compra.MetodoPago);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CompraId, Apellido_1Cliente, Apellido_2Cliente, FechaCompra, nBocadillos, NombreCliente, PrecioTotal, MetodoPago);
        }
    }
}
