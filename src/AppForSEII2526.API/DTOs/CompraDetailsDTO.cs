using System.ComponentModel.DataAnnotations;
using DataType = System.ComponentModel.DataAnnotations.DataType;

namespace AppForSEII2526.API.DTOs
{
    public class CompraDetailsDTO
    {
        public long CompraId { get; set; }
        public string NombreCompleto { get; set; } = "";
        public string Apellidos { get; set; } = "";
        public string MetodoPago { get; set; } = "";
        [DataType(DataType.DateTime)]
        public DateTime Fecha { get; set; }
        [DataType(DataType.Currency)]
        public decimal PrecioTotal { get; set; }
        public IList<CompraBonoItemDTO> Items { get; set; } = new List<CompraBonoItemDTO>();
    }

    public class CompraBonoItemDTO
    {
        public long BonoId { get; set; }
        public string Nombre { get; set; } = "";
        public string? Tipo { get; set; }      // vegano | vegetariano | sin gluten | normal
        [DataType(DataType.Currency)]
        public decimal Pvp { get; set; }       // precio individual
        public int Cantidad { get; set; }
    }
}
