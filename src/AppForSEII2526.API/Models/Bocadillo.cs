using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.Models
{
    public class Bocadillo
    {

        public Bocadillo() { }

        [Key]
        public int Id { get; set; }

        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public String Nombre { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public float PVP {  get; set; }

        [Required]
        public int Stock {  get; set; }

        public Tamanyo TamanyoBocadillo { get; set; }
        
        public TipoPan TipoPan {  get; set; }
        public IList<CompraBocadillo> ComprasDelBocadillo {  get; set; }
        
        public List<ResenyaBocadillo> ResenyaBocadillo { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is Bocadillo bocadillo &&
                   Id == bocadillo.Id &&
                   Nombre == bocadillo.Nombre &&
                   PVP == bocadillo.PVP &&
                   Stock == bocadillo.Stock &&
                   TamanyoBocadillo == bocadillo.TamanyoBocadillo &&
                   EqualityComparer<TipoPan>.Default.Equals(TipoPan, bocadillo.TipoPan) &&
                   EqualityComparer<IList<CompraBocadillo>>.Default.Equals(ComprasDelBocadillo, bocadillo.ComprasDelBocadillo) &&
                   EqualityComparer<List<ResenyaBocadillo>>.Default.Equals(ResenyaBocadillo, bocadillo.ResenyaBocadillo);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Nombre, PVP, Stock, TamanyoBocadillo, TipoPan, ComprasDelBocadillo, ResenyaBocadillo);
        }
    }

    public enum Tamanyo
    {
        Normal,
        Pequenyo
    }
}
