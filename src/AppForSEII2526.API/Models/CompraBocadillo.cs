using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(BocadilloId),
    nameof(CompraId))]
    public class CompraBocadillo
    {

        public CompraBocadillo()
        {

        }
        public CompraBocadillo(Bocadillo bocadillo, int cantidad, Compra compra)
        {
            BocadilloId = bocadillo.Id;
            Cantidad = cantidad;    
            CompraId = compra.CompraId;
            NombreBocadillo = bocadillo.Nombre;
            Precio = bocadillo.PVP;
            TipoPan = bocadillo.TipoPan.Nombre;
            Bocadillo = bocadillo;
            Compra = compra;
        }


        public int BocadilloId { get; set; }

        
        public int Cantidad { get; set; }

        
        public int CompraId {  get; set; }

        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String NombreBocadillo { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public float Precio {  get; set; }

        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String TipoPan {  get; set; }
        
        
        public Bocadillo Bocadillo {  get; set; }
         
        
        public Compra Compra {  get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadillo bocadillo &&
                   BocadilloId == bocadillo.BocadilloId &&
                   Cantidad == bocadillo.Cantidad &&
                   CompraId == bocadillo.CompraId &&
                   NombreBocadillo == bocadillo.NombreBocadillo &&
                   Precio == bocadillo.Precio &&
                   TipoPan == bocadillo.TipoPan &&
                   EqualityComparer<Bocadillo>.Default.Equals(Bocadillo, bocadillo.Bocadillo) &&
                   EqualityComparer<Compra>.Default.Equals(Compra, bocadillo.Compra);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BocadilloId, Cantidad, CompraId, NombreBocadillo, Precio, TipoPan, Bocadillo, Compra);
        }
    }
}
