

namespace AppForSEII2526.API.DTOs.CompraDTOs
{
    public class CompraBocadilloForCreateDTO
    {
        public CompraBocadilloForCreateDTO(){}
        public CompraBocadilloForCreateDTO(
    string nombreCliente,
    string apellido1_cliente,
    string? apellido2_cliente,
    int metodoPagoId,
    IList<CompraBocadilloItemDTO> bocadillosComprados)
{
    NombreCliente = nombreCliente;
    Apellido1_cliente = apellido1_cliente;
    Apellido2_cliente = apellido2_cliente;
    MetodoPagoId = metodoPagoId;
    BocadillosComprados = bocadillosComprados;
}
        
        public string NombreCliente { get; set; }
        
        public string Apellido1_cliente { get; set; }
        public string? Apellido2_cliente { get; set; }
        
        public int MetodoPagoId { get; set; }
        public IList<CompraBocadilloItemDTO> BocadillosComprados { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadilloForCreateDTO dTO &&
                   NombreCliente == dTO.NombreCliente &&
                   Apellido1_cliente == dTO.Apellido1_cliente &&
                   Apellido2_cliente == dTO.Apellido2_cliente &&
                   MetodoPagoId == dTO.MetodoPagoId &&
                   //EqualityComparer<MetodoPago>.Default.Equals(MetodoPago, dTO.MetodoPago) &&
                   (BocadillosComprados ?? new List<CompraBocadilloItemDTO>())
    .SequenceEqual(dTO.BocadillosComprados ?? new List<CompraBocadilloItemDTO>());
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreCliente, Apellido1_cliente, Apellido2_cliente, MetodoPagoId, BocadillosComprados);
        }
    }
}
