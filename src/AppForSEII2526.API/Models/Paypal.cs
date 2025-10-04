namespace AppForSEII2526.API.Models
{
    public sealed class Paypal: MetodoPago
    {
        public override int Id { get; protected set; }
        public override string Metodo => "Paypal";
    }
}
