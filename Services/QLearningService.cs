using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace OnboardingAPI.Services
{
    public class QLearningService : IQLearningService
    {
        private readonly string _connectionString;

        public QLearningService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Executa o algoritmo Q-Learning com base nas interações e retorna a Tabela Q aprendida.
        /// </summary>
        /// <param name="interacoesAmbiente">O histórico de interações para treinar o modelo.</param>
        /// <returns>Um dicionário representando a Tabela Q: (estado, ação) -> valor Q.</returns>
        public async Task<Dictionary<Tuple<int, int>, double>> ExecutarEObterTabelaQ(List<Interacao> interacoesAmbiente)
        {
            var Q = new Dictionary<Tuple<int, int>, double>();

            if (interacoesAmbiente == null || !interacoesAmbiente.Any())
            {
                return Q; // Retorna tabela vazia se não há dados
            }

            // Deriva todos os estados e ações únicos do ambiente
            var todosEstados = interacoesAmbiente
                .Select(i => i.EstadoAtualId)
                .Concat(interacoesAmbiente.Select(i => i.ProximoEstadoId))
                .Where(id => id != 0)
                .Distinct()
                .ToList();

            var todasAcoes = interacoesAmbiente
                .Select(i => i.AcaoTomadaId)
                .Where(id => id != 0)
                .Distinct()
                .ToList();

            if (!todosEstados.Any() || !todasAcoes.Any())
            {
                return Q; // Retorna tabela vazia se não há estados/ações válidos
            }

            // Parâmetros do Q-Learning
            const double alpha = 0.1;   // Taxa de aprendizado
            const double gamma = 0.9;   // Fator de desconto
            const int episodios = 500; // Número de episódios de treinamento
            const int maxPassosPorEpisodio = 100;

            var rand = new Random();

            // Inicializa a Tabela Q com zeros
            foreach (var estadoId in todosEstados)
            {
                foreach (var acaoId in todasAcoes)
                {
                    Q[Tuple.Create(estadoId, acaoId)] = 0.0;
                }
            }

            // Loop de treinamento
            for (int episodio = 0; episodio < episodios; episodio++)
            {
                var estadoAtualId = todosEstados[rand.Next(todosEstados.Count)];

                for (int passo = 0; passo < maxPassosPorEpisodio; passo++)
                {
                    // Ação escolhida aleatoriamente para exploração
                    var acaoEscolhidaId = todasAcoes[rand.Next(todasAcoes.Count)];

                    // Simula a transição com base nos dados reais
                    var transicaoReal = interacoesAmbiente.FirstOrDefault(i =>
                        i.EstadoAtualId == estadoAtualId && i.AcaoTomadaId == acaoEscolhidaId);

                    double recompensa;
                    int proximoEstadoId;

                    if (transicaoReal != null)
                    {
                        recompensa = transicaoReal.RecompensaRecebida;
                        proximoEstadoId = transicaoReal.ProximoEstadoId;
                    }
                    else
                    {
                        recompensa = -1.0; // Penalidade por ação inválida/indefinida
                        proximoEstadoId = estadoAtualId; // Permanece no mesmo estado
                    }

                    // Calcula o valor Q máximo para o próximo estado
                    double qMaxProximo = 0.0;
                    if (proximoEstadoId != 0)
                    {
                        qMaxProximo = todasAcoes
                            .Select(a => Q.TryGetValue(Tuple.Create(proximoEstadoId, a), out double val) ? val : 0.0)
                            .DefaultIfEmpty(0.0)
                            .Max();
                    }

                    // Atualiza a Tabela Q usando a equação de Bellman
                    var qKey = Tuple.Create(estadoAtualId, acaoEscolhidaId);
                    double qValorAtual = Q[qKey];
                    Q[qKey] = qValorAtual + alpha * (recompensa + gamma * qMaxProximo - qValorAtual);

                    estadoAtualId = proximoEstadoId;

                    // Condição de término do episódio
                    if (estadoAtualId == 0 || !interacoesAmbiente.Any(i => i.EstadoAtualId == estadoAtualId))
                    {
                        break;
                    }
                }
            }

            // Retorna a Tabela Q aprendida
            return await Task.FromResult(Q);
        }
        
        public async Task<AnaliseQLearning> Executar(List<Interacao> interacoesAmbiente)
        {
            var qTable = await ExecutarEObterTabelaQ(interacoesAmbiente);
            double valorQMedio = qTable.Any() ? qTable.Values.Average() : 0.0;
            double dispersaoQ = qTable.Any() ? qTable.Values.Max() - qTable.Values.Min() : 0.0;

            return new AnaliseQLearning
            {
                Semana = $"{DateTime.Now.Year}-S{ISOWeek.GetWeekOfYear(DateTime.Now)}",
                ValorQ = valorQMedio,
                TaxaConvergencia = dispersaoQ
            };
        }
    }
}