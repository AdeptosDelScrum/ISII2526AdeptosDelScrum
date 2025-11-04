using Humanizer.DateTimeHumanizeStrategy;
using System.Drawing;
using DataType = System.ComponentModel.DataAnnotations.DataType;
namespace AppForSEII2526.API.DTOs
{
    public class BocadilloDTO
    {
        public BocadilloDTO() { }
        public BocadilloDTO(string nombre, Tamanyo tamanyo,String tipopan, float precio) { 
            Name = nombre;
            TamanyoBocadillo = tamanyo;
            TipoPan = tipopan;
            PVP = precio;
        
        }
        public int Id { get; set; }

        [StringLength(30, ErrorMessage = "El nombre no puede tener más de 30 caracteres.")]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")]
        public string Name { get; set; }

        public String TipoPan { get; set; }

        [DataType(DataType.Currency)]
        [Display(Name = "Total")]
        public float PVP { get; set; }

        public Tamanyo TamanyoBocadillo { get; set; }


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
