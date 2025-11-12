


namespace AppForSEII2526.API.DTOs.CompraDTOs
{
    public class CompraBocadilloItemDTO
    {
        public CompraBocadilloItemDTO(string nombre, float precio, string tipoPan)
        {
            Nombre = nombre;
            Precio = precio;
            TipoPan = tipoPan;

        }

        public string Nombre { get; set; }
        public float Precio { get; set; }
        public string TipoPan { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is CompraBocadilloItemDTO dTO &&
                   Nombre == dTO.Nombre &&
                   Precio == dTO.Precio &&
                   TipoPan == dTO.TipoPan;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Nombre, Precio, TipoPan);
        }
    }
}