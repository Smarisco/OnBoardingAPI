using OnboardingAPI.Dto;
using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingAPI.Services
{
    public class AnaliseService : IAnaliseService
    {
        private readonly IAlgoritmoGeneticoService _geneticoService;
        private readonly IQLearningService _qLearningService;
        private readonly IInteracaoService _interacaoService;
        private readonly string _connectionString;

        public AnaliseService(IConfiguration config,
                              IAlgoritmoGeneticoService geneticoService,
                              IQLearningService qLearningService,
                              IInteracaoService interacaoService)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
            _geneticoService = geneticoService;
            _qLearningService = qLearningService;
            _interacaoService = interacaoService;
        }

        public async Task<ResultadoAnaliseDto> RodarAnaliseSemanalAsync(DateTime dataReferencia)
        {
            var inicioSemana = dataReferencia.Date.AddDays(-(int)dataReferencia.DayOfWeek + (int)DayOfWeek.Monday);
            var fimSemana = inicioSemana.AddDays(6);
            var semana = CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
                dataReferencia, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday).ToString();

            var interacoes = (await _interacaoService.ObterPorPeriodoAsync(inicioSemana, fimSemana))?.ToList() ?? new List<Interacao>();

            if (!interacoes.Any())
            {
                return new ResultadoAnaliseDto { Semana = semana, Detalhes = "Nenhuma interação encontrada no período." };
            }

            var resultadoGenetico = await _geneticoService.Executar(interacoes);
            var qTable = await _qLearningService.ExecutarEObterTabelaQ(interacoes);
            var resultadoEstrategiaQL = GerarEAvaliarMelhorSequenciaDoQLearning(qTable, interacoes);

            string melhorAlgoritmo;
            if (resultadoGenetico.MelhorFitness > resultadoEstrategiaQL.Fitness)
            {
                melhorAlgoritmo = "Algoritmo Genético";
            }
            else if (resultadoEstrategiaQL.Fitness > resultadoGenetico.MelhorFitness)
            {
                melhorAlgoritmo = "Q-Learning";
            }
            else
            {
                melhorAlgoritmo = "Empate Técnico";
            }

            var resultado = new ResultadoAnaliseDto
            {
                Semana = semana,
                DataAnalise = DateTime.Now,
                PeriodoInicio = inicioSemana,
                PeriodoFim = fimSemana,
                MediaFitnessGenetico = resultadoGenetico.MediaFitness,
                MelhorFitnessGenetico = resultadoGenetico.MelhorFitness,
                FitnessDaMelhorEstrategiaQLearning = resultadoEstrategiaQL.Fitness,
                MelhorAlgoritmo = melhorAlgoritmo,
                Detalhes = $"Análise comparativa de Fitness. AG: {resultadoGenetico.MelhorFitness:F2}. QL: {resultadoEstrategiaQL.Fitness:F2}."
            };

            await SalvarResultadoAsync(resultado);
            return resultado;
        }

        private (double Fitness, List<int> SequenciaAcoes) GerarEAvaliarMelhorSequenciaDoQLearning(
            Dictionary<Tuple<int, int>, double> qTable,
            List<Interacao> interacoes)
        {
            if (qTable == null || !qTable.Any())
                return (0, new List<int>());

            double fitnessTotal = 0;
            var sequenciaAcoes = new List<int>();

            int estadoAtual = interacoes.OrderBy(i => i.DataInteracao).First().EstadoAtualId;
            var estadosVisitados = new HashSet<int> { estadoAtual };

            for (int i = 0; i < 20; i++)
            {
                var melhorAcaoParaEstado = qTable
                    .Where(q => q.Key.Item1 == estadoAtual)
                    .OrderByDescending(q => q.Value)
                    .Select(q => (int?)q.Key.Item2)
                    .FirstOrDefault();

                if (melhorAcaoParaEstado == null) break;

                int acaoEscolhida = melhorAcaoParaEstado.Value;

                var transicaoReal = interacoes
                    .Where(intr => intr.EstadoAtualId == estadoAtual && intr.AcaoTomadaId == acaoEscolhida)
                    .OrderByDescending(intr => intr.RecompensaRecebida)
                    .FirstOrDefault();

                if (transicaoReal == null) break;

                fitnessTotal += transicaoReal.RecompensaRecebida;
                sequenciaAcoes.Add(acaoEscolhida);
                estadoAtual = transicaoReal.ProximoEstadoId;

                if (!estadosVisitados.Add(estadoAtual)) break;
            }

            return (fitnessTotal, sequenciaAcoes);
        }

        public async Task SalvarResultadoAsync(ResultadoAnaliseDto resultado)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var query = @"INSERT INTO ResultadoAnalise
                              (Semana, DataAnalise, PeriodoInicio, PeriodoFim, MediaFitnessGenetico, MelhorFitnessGenetico, FitnessDaMelhorEstrategiaQLearning, MelhorAlgoritmo, Detalhes)
                              VALUES
                              (@Semana, @DataAnalise, @PeriodoInicio, @PeriodoFim, @MediaFitnessGenetico, @MelhorFitnessGenetico, @FitnessDaMelhorEstrategiaQLearning, @MelhorAlgoritmo, @Detalhes)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Semana", resultado.Semana);
                    command.Parameters.AddWithValue("@DataAnalise", resultado.DataAnalise);
                    command.Parameters.AddWithValue("@PeriodoInicio", resultado.PeriodoInicio);
                    command.Parameters.AddWithValue("@PeriodoFim", resultado.PeriodoFim);
                    command.Parameters.AddWithValue("@MediaFitnessGenetico", resultado.MediaFitnessGenetico);
                    command.Parameters.AddWithValue("@MelhorFitnessGenetico", resultado.MelhorFitnessGenetico);
                    command.Parameters.AddWithValue("@FitnessDaMelhorEstrategiaQLearning", resultado.FitnessDaMelhorEstrategiaQLearning);
                    command.Parameters.AddWithValue("@MelhorAlgoritmo", resultado.MelhorAlgoritmo);
                    command.Parameters.AddWithValue("@Detalhes", resultado.Detalhes);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}