
namespace AppForSEII2526.API.DTOs.CompraDTOs
{
    public class CompraBocadilloForCreateDTO
    {
        public CompraBocadilloForCreateDTO(string nombre_cliente, string apellido1_cliente, string? apellido2_cliente, MetodoPago metodoPago, IList<CompraBocadilloItemDTO> bocadillosComprados) 
        {
            NombreCliente = nombre_cliente ?? throw new ArgumentNullException(nameof(nombre_cliente)); ;
            Apellido1_cliente = apellido1_cliente ?? throw new ArgumentNullException(nameof(apellido1_cliente)); ;
            Apellido2_cliente = apellido2_cliente;
            MetodoPago = metodoPago ?? throw new ArgumentNullException(nameof(metodoPago)); ;
            BocadillosComprados = bocadillosComprados ?? throw new ArgumentNullException(nameof(bocadillosComprados)); 
        
        }
        [Required]
        public string NombreCliente { get; set; }
        [Required]
        public string Apellido1_cliente { get; set; }
        public string? Apellido2_cliente { get; set; }
        [Required]
        public MetodoPago MetodoPago { get; set; }
        public IList<CompraBocadilloItemDTO> BocadillosComprados { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadilloForCreateDTO dTO &&
                   NombreCliente == dTO.NombreCliente &&
                   Apellido1_cliente == dTO.Apellido1_cliente &&
                   Apellido2_cliente == dTO.Apellido2_cliente &&
                   MetodoPago == dTO.MetodoPago &&
                   EqualityComparer<IList<CompraBocadilloItemDTO>>.Default.Equals(BocadillosComprados, dTO.BocadillosComprados);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NombreCliente, Apellido1_cliente, Apellido2_cliente, MetodoPago, BocadillosComprados);
        }
    }
}
