using DataType = System.ComponentModel.DataAnnotations.DataType;
namespace AppForSEII2526.API.DTOs
{
    public class BocadilloDTO
    {
        public int Id { get; set; }

        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string Name { get; set; }

        public enum tamanyo { Normal, Pequenyo }

        public TipoPan TipoPan { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public float PVP { get; set; }
    }
}
