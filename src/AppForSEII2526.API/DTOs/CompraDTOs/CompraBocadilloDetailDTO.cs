using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.DTOs.CompraDTOs
{

    public class CompraBocadilloDetailDTO : CompraBocadilloForCreateDTO
    {
        public CompraBocadilloDetailDTO(int id, DateTime fecha, float precio, string nombre_cliente, string apellido1_cliente, string? apellido2_cliente, MetodoPago metodoPago, IList<CompraBocadilloItemDTO> bocadillosComprados)
         : base(nombre_cliente,
                   apellido1_cliente,
                   apellido2_cliente,
                   metodoPago,
                   bocadillosComprados)
        {
            Id = id;
            Fecha = fecha;
            Precio = precio;

        }

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public float Precio { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadilloDetailDTO dTO &&
                   base.Equals(obj) &&
                   Precio == dTO.Precio &&
                   Id == dTO.Id &&
                   Fecha == dTO.Fecha;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(base.GetHashCode(), Id, Fecha, Precio);
        }
    }
}