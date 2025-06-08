namespace OnboardingAPI.Models
{
    public class Interacao
    {
        public int Id { get; set; }
        public int EstagiarioId { get; set; }
        public int EstadoAtualId { get; set; }
        public int AcaoTomadaId { get; set; }
        public int ProximoEstadoId { get; set; }
        public double RecompensaRecebida { get; set; }
        public DateTime DataInteracao { get; set; }
    }
}
