using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AppForSEII2526.API.DTOs.CompraDTOs
{
    public class CompraBocadilloDetailDTO
    {
        public CompraBocadilloDetailDTO() { }

        public CompraBocadilloDetailDTO(
            int id,
            DateTime fecha,
            float precio,
            string nombreCliente,
            string apellido1Cliente,
            string? apellido2Cliente,
            int metodoPagoId,
            int cantidadTotal,
            IList<CompraBocadilloItemDTO> bocadillosComprados)
                {
                    Id = id;
                    Fecha = fecha;
                    Precio = precio;
                    NombreCliente = nombreCliente;
                    Apellido1_cliente = apellido1Cliente;
                    Apellido2_cliente = apellido2Cliente;
                    
                    MetodoPagoId = metodoPagoId;
                    CantidadTotal = cantidadTotal;
                    BocadillosComprados = bocadillosComprados;
                }


        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public float Precio { get; set; }

        public string NombreCliente { get; set; }
        public string Apellido1_cliente { get; set; }
        public string? Apellido2_cliente { get; set; }

        public int MetodoPagoId { get; set; }
        public int CantidadTotal { get; set; }

        public IList<CompraBocadilloItemDTO> BocadillosComprados { get; set; }
            = new List<CompraBocadilloItemDTO>();

        public override bool Equals(object? obj)
    => Equals(obj as CompraBocadilloDetailDTO);

        public bool Equals(CompraBocadilloDetailDTO? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Id == other.Id
                && Fecha.Date == other.Fecha.Date
                && Precio == other.Precio
                && NombreCliente == other.NombreCliente
                && Apellido1_cliente == other.Apellido1_cliente
                && Apellido2_cliente == other.Apellido2_cliente
                && MetodoPagoId == other.MetodoPagoId
                && CantidadTotal == other.CantidadTotal
                && BocadillosComprados.SequenceEqual(other.BocadillosComprados);
        }


        public override int GetHashCode()
        {
            var hash = new HashCode();

            hash.Add(Id);
            hash.Add(Fecha.Date);
            hash.Add(Precio);
            hash.Add(NombreCliente);
            hash.Add(Apellido1_cliente);
            hash.Add(Apellido2_cliente);
            hash.Add(MetodoPagoId);
            hash.Add(CantidadTotal);

            foreach (var item in BocadillosComprados)
                hash.Add(item);

            return hash.ToHashCode();
        }

    }
}
