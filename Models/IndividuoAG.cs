namespace OnboardingAPI.Models
{
    public class IndividuoAG
    {
        public Guid Id { get; private set; }
        public List<int> SequenciaAcoes { get; set; } // Os "genes" = IDs das atividades/ações
        public int EstadoInicialOriginal { get; set; } // De onde a sequência original começou
        public double Fitness { get; set; } // Calculado a partir das RecompensasReais

        public IndividuoAG()
        {
            Id = Guid.NewGuid();
            SequenciaAcoes = new List<int>();
            Fitness = double.MinValue;
        }
    }
}
