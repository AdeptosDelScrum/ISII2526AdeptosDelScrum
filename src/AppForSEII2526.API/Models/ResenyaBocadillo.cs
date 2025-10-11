namespace AppForSEII2526.API.Models
{
    [PrimaryKey(nameof(BocadilloId),nameof(ResenyaId))]
    public class ResenyaBocadillo
    {
        public int BocadilloId { get; set; }
        public int ResenyaId { get; set; }

        [Required, Range(1, 10, ErrorMessage = "La puntuación del bocadillo tiene un rango del 1 al 10")]
        [Display(Name = "Puntuación del bocadillo")]
        public int Puntuacion { get; set; }

        public Resenya Resenya { get; set; }

        public Bocadillo Bocadillo { get; set; }

    }
}
