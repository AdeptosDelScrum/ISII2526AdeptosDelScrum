using DataType = System.ComponentModel.DataAnnotations.DataType;
namespace AppForSEII2526.API.DTOs
{
    public class BocadilloDTO
    {
        public BocadilloDTO(int id, string name, string tipoPan, float pVP)
        {
            Id = id;
            Name = name;
            TipoPan = tipoPan;
            PVP = pVP;
        }

        public int Id { get; set; }

        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string Name { get; set; }

        public enum tamanyo { Normal, Pequenyo }

        public String TipoPan { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public float PVP { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is BocadilloDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   TipoPan == dTO.TipoPan &&
                   PVP == dTO.PVP;
        }
    }
}
