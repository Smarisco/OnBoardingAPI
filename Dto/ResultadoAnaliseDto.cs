namespace OnboardingAPI.Dto
{
    public class ResultadoAnaliseDto
    {
        public string Semana { get; set; }
        public DateTime DataAnalise { get; set; }
        public DateTime PeriodoInicio { get; set; }
        public DateTime PeriodoFim { get; set; }

        // Métricas do Algoritmo Genético
        public double MediaFitnessGenetico { get; set; }
        public double MelhorFitnessGenetico { get; set; }

        // Métrica Comparável do Q-Learning
        public double FitnessDaMelhorEstrategiaQLearning { get; set; } // << ADICIONE ESTA LINHA

        // Resultado da Comparação
        public string MelhorAlgoritmo { get; set; }
        public string Detalhes { get; set; }
    }

}
