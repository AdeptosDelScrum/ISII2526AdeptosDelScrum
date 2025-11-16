




namespace AppForSEII2526.API.DTOs.CompraDTOs
{
    public class CompraBocadilloItemDTO:IEquatable<CompraBocadilloItemDTO>
    {
        public CompraBocadilloItemDTO(
            )
        {


        }
        public CompraBocadilloItemDTO(string nombre, float precio, string tipoPan, int cantidad)
        {
            Nombre = nombre;
            Precio = precio;
            TipoPan = tipoPan;
            Cantidad = cantidad;

        }

        public string Nombre { get; set; }
        public float Precio { get; set; }
        public string TipoPan { get; set; }
        public int Cantidad { get; set; }

        public override bool Equals(object? obj) => Equals(obj as CompraBocadilloItemDTO);

        public bool Equals(CompraBocadilloItemDTO? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return Nombre == other.Nombre
                && Precio == other.Precio
                && TipoPan == other.TipoPan
                && Cantidad == other.Cantidad;
        }

        public override int GetHashCode()
            => HashCode.Combine(Nombre, Precio, TipoPan, Cantidad);
    }
}
