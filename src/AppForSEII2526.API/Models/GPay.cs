namespace AppForSEII2526.API.Models
{
    public sealed class GPay: MetodoPago
    {
        public override int Id { get; protected set; }
        public override string Metodo => "GPay";
    }
}
