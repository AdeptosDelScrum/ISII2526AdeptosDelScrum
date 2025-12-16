using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.DTOs.CompraDTOs
{

    public class CompraBocadilloDetailDTO : CompraBocadilloForCreateDTO
    {
        public CompraBocadilloDetailDTO(int id, DateTime fecha, float precio, string nombre_cliente, string apellido1_cliente, string? apellido2_cliente, int cantidad, int metodoPago, IList<CompraBocadilloItemDTO> bocadillosComprados)
         : base(nombre_cliente,
                   apellido1_cliente,
                   apellido2_cliente,
                   metodoPago,
                   bocadillosComprados
                   )
        {
            Id = id;
            Fecha = fecha;
            Precio = precio;
        }
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public float Precio { get; set; }

        public override bool Equals(object? obj) => Equals(obj as CompraBocadilloDetailDTO);

        public bool Equals(CompraBocadilloDetailDTO? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            
            if (!base.Equals(other)) return false;

            
            return Id == other.Id
                && Fecha.Date == other.Fecha.Date
                && Precio == other.Precio;
        }

        public override int GetHashCode()
            => HashCode.Combine(base.GetHashCode(), Id, Fecha, Precio);
    }
}
