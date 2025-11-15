using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.Models
{
    public class Compra
    {

        public Compra() { }
        public Compra(string nombre, string apellido1, string apellido2, DateTime fecha, int cantidad, MetodoPago metodoPago, IList<CompraBocadillo> bocadillosComprados)
        {
            User.NombreCliente = nombre;
            User.Apellido1_Cliente = apellido1;
            User.Apellido2_Cliente = apellido2;
            FechaCompra = fecha;
            nBocadillos = cantidad;
            PrecioTotal = bocadillosComprados.Sum(ci => ci.Precio * ci.Cantidad);
            MetodoPago = metodoPago;
            BocadillosComprados = bocadillosComprados;
        }

        [Key]
        public int CompraId { get; set; }
        public ApplicationUser User { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FechaCompra {  get; set; }

        
        public int nBocadillos {  get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public float PrecioTotal {  get; set; }

        [Required]
        public MetodoPago MetodoPago { get; set; }

        
        
        public IList<CompraBocadillo> BocadillosComprados { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Compra compra &&
                   CompraId == compra.CompraId &&
                   FechaCompra == compra.FechaCompra &&
                   nBocadillos == compra.nBocadillos &&
                   PrecioTotal == compra.PrecioTotal &&
                   EqualityComparer<MetodoPago>.Default.Equals(MetodoPago, compra.MetodoPago) &&
                   EqualityComparer<IList<CompraBocadillo>>.Default.Equals(BocadillosComprados, compra.BocadillosComprados);
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(CompraId);
            hash.Add(FechaCompra);
            hash.Add(nBocadillos);
            hash.Add(PrecioTotal);
            hash.Add(MetodoPago);
            hash.Add(BocadillosComprados);
            return hash.ToHashCode();
        }
    }
}
