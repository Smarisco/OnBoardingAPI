using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using System.Globalization;

namespace OnboardingAPI.Services
{
    public class AlgoritmoGeneticoService : IAlgoritmoGeneticoService
    {
            // Seus métodos e construtor existentes (não mude nada aqui)
            private readonly string _connectionString;

            public AlgoritmoGeneticoService(IConfiguration configuration)
            {
                _connectionString = configuration.GetConnectionString("DefaultConnection");
            }

            private const double PENALIDADE_SEQUENCIA_INCOMPLETA_FITNESS = -1000.0;

            public async Task<AnaliseAlgoritmoGenetico> Executar(List<Interacao> interacoesDoEstagiario)
            {
            string semanaAtual = $"{DateTime.Now.Year}-S{CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)}";
            if (interacoesDoEstagiario == null || !interacoesDoEstagiario.Any())
                {
                    return new AnaliseAlgoritmoGenetico { Semana = semanaAtual };
                }

                // Parâmetros do AG
                int numeroDeGeracoes = 50;
                Dictionary<Tuple<int, int>, double> qTable = null; // Opcional, pode ser obtido se necessário

                // PASSO 1: Chama o método auxiliar para obter a população inicial
                var populacaoInicial = await ObterPopulacaoInicialObservadaAsync(interacoesDoEstagiario);

                if (populacaoInicial == null || !populacaoInicial.Any())
                {
                    return new AnaliseAlgoritmoGenetico { Semana = semanaAtual };
                }

                List<IndividuoAG> geracaoAtual = populacaoInicial.ToList();

                // PASSO 2: Loop de gerações, chamando o método auxiliar para evoluir
                for (int g = 0; g < numeroDeGeracoes; g++)
                {
                    var proximaGeracao = GerarNovaGeracaoDeterministaAsync(geracaoAtual, interacoesDoEstagiario, qTable)?.ToList();

                    if (proximaGeracao == null || !proximaGeracao.Any())
                    {
                        break; // Interrompe a evolução se não houver progresso
                    }
                    geracaoAtual = proximaGeracao;
                }

                // PASSO 3: Retorna o objeto de análise com os resultados finais
                var analiseFinal = new AnaliseAlgoritmoGenetico
                {
                    Semana = semanaAtual,
                    MelhorFitness = geracaoAtual.Any() ? geracaoAtual.Max(i => i.Fitness) : 0,
                    MediaFitness = geracaoAtual.Any() ? geracaoAtual.Average(i => i.Fitness) : 0,
                    QtdIndividuos = geracaoAtual.Count
                };

                
                await SalvarAnaliseSemanalAsync(analiseFinal);

                return analiseFinal;
            }


            public async Task<IEnumerable<IndividuoAG>> ObterPopulacaoInicialObservadaAsync(IEnumerable<Interacao> interacoesDoEstagiario)
            {
                var populacao = new List<IndividuoAG>();
                if (interacoesDoEstagiario == null || !interacoesDoEstagiario.Any())
                {
                    return populacao;
                }
                var interacoesOrdenadas = interacoesDoEstagiario.OrderBy(i => i.DataInteracao).ToList();
                var visitados = new HashSet<int>();
                foreach (var interacaoInicial in interacoesOrdenadas)
                {
                    if (visitados.Contains(interacaoInicial.Id)) continue;
                    var individuo = new IndividuoAG { EstadoInicialOriginal = interacaoInicial.EstadoAtualId };
                    var sequenciaAtual = new List<Interacao> { interacaoInicial };
                    visitados.Add(interacaoInicial.Id);
                    int estadoCorrente = interacaoInicial.ProximoEstadoId;
                    bool caminhoContinuou = true;
                    while (caminhoContinuou)
                    {
                        var proximaInteracaoNoCaminho = interacoesOrdenadas
                            .FirstOrDefault(i => i.EstadoAtualId == estadoCorrente && !visitados.Contains(i.Id) && i.DataInteracao > sequenciaAtual.Last().DataInteracao);
                        if (proximaInteracaoNoCaminho != null)
                        {
                            sequenciaAtual.Add(proximaInteracaoNoCaminho);
                            visitados.Add(proximaInteracaoNoCaminho.Id);
                            estadoCorrente = proximaInteracaoNoCaminho.ProximoEstadoId;
                        }
                        else
                        {
                            caminhoContinuou = false;
                        }
                    }
                    if (sequenciaAtual.Any())
                    {
                        individuo.SequenciaAcoes = sequenciaAtual.Select(i => i.AcaoTomadaId).ToList();
                        CalcularFitnessParaSequenciaObservada(individuo, sequenciaAtual);
                        populacao.Add(individuo);
                    }
                }
                return await Task.FromResult(populacao.DistinctBy(p => string.Join(",", p.SequenciaAcoes)).ToList());
            }
        //
            public IEnumerable<IndividuoAG> GerarNovaGeracaoDeterministaAsync(IEnumerable<IndividuoAG> populacaoAtual, IEnumerable<Interacao> interacoesDoEstagiario, Dictionary<Tuple<int, int>, double> qTable)
            {
                var novaPopulacao = new List<IndividuoAG>();
                var populacaoOrdenada = populacaoAtual.OrderByDescending(ind => ind.Fitness).ToList();
                if (!populacaoOrdenada.Any()) return novaPopulacao;
                int qtdElite = (int)(populacaoOrdenada.Count * 0.20);
                if (qtdElite == 0 && populacaoOrdenada.Any()) qtdElite = 1;
                for (int i = 0; i < qtdElite && i < populacaoOrdenada.Count; i++)
                {
                    novaPopulacao.Add(ClonarIndividuo(populacaoOrdenada[i]));
                }
                int indicePai1 = 0;
                int indicePai2 = populacaoOrdenada.Count - 1;
                while (novaPopulacao.Count < populacaoOrdenada.Count && indicePai1 < indicePai2)
                {
                    var pai1 = populacaoOrdenada[indicePai1];
                    var pai2 = populacaoOrdenada[indicePai2];
                    IndividuoAG filho = CruzamentoPontoFixo(pai1, pai2);
                    CalcularFitnessParaNovaSequencia(filho, interacoesDoEstagiario);
                    novaPopulacao.Add(filho);
                    if (novaPopulacao.Count < populacaoOrdenada.Count)
                    {
                        IndividuoAG filho2 = CruzamentoPontoFixo(pai2, pai1);
                        CalcularFitnessParaNovaSequencia(filho2, interacoesDoEstagiario);
                        novaPopulacao.Add(filho2);
                    }
                    indicePai1++;
                    indicePai2--;
                }
                if (qTable != null && qTable.Any())
                {
                    foreach (var individuoMutante in novaPopulacao)
                    {
                        MutacaoDeterministaGuiadaPorQTable(individuoMutante, qTable, interacoesDoEstagiario);
                        CalcularFitnessParaNovaSequencia(individuoMutante, interacoesDoEstagiario);
                    }
                }
                int indiceEliteExtra = 0;
                while (novaPopulacao.Count < populacaoOrdenada.Count && indiceEliteExtra < populacaoOrdenada.Count)
                {
                    novaPopulacao.Add(ClonarIndividuo(populacaoOrdenada[indiceEliteExtra++]));
                }
                return novaPopulacao.Take(populacaoOrdenada.Count);
            }

            public async Task SalvarAnaliseSemanalAsync(AnaliseAlgoritmoGenetico analise)
            {
                Console.WriteLine($"Serviço AG (Determinístico): Salvando análise da semana {analise.Semana} - Melhor Fitness: {analise.MelhorFitness}, Média: {analise.MediaFitness}, Qtd: {analise.QtdIndividuos}");
                await Task.Delay(50); // Simula I/O para o banco de dados
            }

            
            private void CalcularFitnessParaSequenciaObservada(IndividuoAG individuo, List<Interacao> interacoesDaSequencia)
            {
                individuo.Fitness = interacoesDaSequencia.Sum(i => i.RecompensaRecebida);
            }

            private void CalcularFitnessParaNovaSequencia(IndividuoAG individuo, IEnumerable<Interacao> todasInteracoesEstagiario)
            {
                individuo.Fitness = 0;
                int estadoAtual = individuo.EstadoInicialOriginal;
                double recompensaTotal = 0;
                foreach (int acaoId in individuo.SequenciaAcoes)
                {
                    var transicoesPossiveis = todasInteracoesEstagiario.Where(i => i.EstadoAtualId == estadoAtual && i.AcaoTomadaId == acaoId).ToList();
                    if (transicoesPossiveis.Any())
                    {
                        recompensaTotal += transicoesPossiveis.Average(t => t.RecompensaRecebida);
                        estadoAtual = transicoesPossiveis.OrderByDescending(t => t.RecompensaRecebida).First().ProximoEstadoId;
                    }
                    else
                    {
                        recompensaTotal += PENALIDADE_SEQUENCIA_INCOMPLETA_FITNESS;
                        break;
                    }
                }
                individuo.Fitness = recompensaTotal;
            }

            private IndividuoAG ClonarIndividuo(IndividuoAG original)
        {
            return new IndividuoAG
            {
                SequenciaAcoes = new List<int>(original.SequenciaAcoes),
                EstadoInicialOriginal = original.EstadoInicialOriginal,
                Fitness = original.Fitness // Fitness é copiado, mas para filhos de crossover é recalculado
            };
        }

        private IndividuoAG CruzamentoPontoFixo(IndividuoAG pai1, IndividuoAG pai2)
        {
            var filho = new IndividuoAG
            {
                // Regra Determinística para EstadoInicial: ex, do pai com maior fitness, ou sempre do pai1.
                EstadoInicialOriginal = pai1.Fitness >= pai2.Fitness ? pai1.EstadoInicialOriginal : pai2.EstadoInicialOriginal
            };

            int meioPai1 = pai1.SequenciaAcoes.Count / 2;
            // int meioPai2 = pai2.SequenciaAcoes.Count / 2; // Não usado no exemplo do paper

            // Conforme exemplo do seu paper: GenesFilho[j] = j < meio ? pai1.Genes[j] : pai2.Genes[j];
            // Isso implica que o tamanho do filho é o tamanho do pai1.
            // Vamos garantir que o filho tenha uma sequência.
            if (pai1.SequenciaAcoes.Any())
            {
                for (int j = 0; j < pai1.SequenciaAcoes.Count; j++)
                {
                    if (j < meioPai1)
                    {
                        filho.SequenciaAcoes.Add(pai1.SequenciaAcoes[j]);
                    }
                    else if (j < pai2.SequenciaAcoes.Count) // Garante que não estouramos o pai2
                    {
                        filho.SequenciaAcoes.Add(pai2.SequenciaAcoes[j]);
                    }
                    // Se pai2 for menor que pai1, o restante da sequência do filho não será preenchido por pai2.
                    // Poderia preencher com o restante de pai1, ou deixar como está (sequência menor).
                    // Para manter o tamanho de pai1:
                    else
                    {
                        filho.SequenciaAcoes.Add(pai1.SequenciaAcoes[j]);
                    }
                }
            }
            else if (pai2.SequenciaAcoes.Any()) // Se pai1 era vazio, pega de pai2
            {
                filho.SequenciaAcoes.AddRange(pai2.SequenciaAcoes);
            }
            // Se ambos os pais forem vazios, o filho será vazio. Isso é ok.

            return filho;
        }

        private void MutacaoDeterministaGuiadaPorQTable(IndividuoAG individuo, Dictionary<Tuple<int, int>, double> qTable, IEnumerable<Interacao> interacoesEstagiario)
        {
            if (!individuo.SequenciaAcoes.Any()) return;

            int estadoCorrenteParaMutacao = individuo.EstadoInicialOriginal;
            bool fitnessRecalculadoNecessario = false;

            for (int i = 0; i < individuo.SequenciaAcoes.Count; i++)
            {
                int acaoAtual = individuo.SequenciaAcoes[i];
                var chaveAtual = Tuple.Create(estadoCorrenteParaMutacao, acaoAtual);
                double qValorAtual = qTable.ContainsKey(chaveAtual) ? qTable[chaveAtual] : double.MinValue;

                // Encontrar a melhor ação alternativa da Q-Table para o estadoCorrenteParaMutacao
                int melhorAcaoAlternativa = acaoAtual;
                double qValorMelhorAlternativa = qValorAtual;

                foreach (var qEntry in qTable.Where(q => q.Key.Item1 == estadoCorrenteParaMutacao))
                {
                    if (qEntry.Value > qValorMelhorAlternativa)
                    {
                        qValorMelhorAlternativa = qEntry.Value;
                        melhorAcaoAlternativa = qEntry.Key.Item2;
                    }
                }

                // Regra de Mutação Determinística: Se uma ação alternativa é significativamente melhor (ex: > 10%)
                // e DIFERENTE da ação atual, substitui.
                if (melhorAcaoAlternativa != acaoAtual && qValorMelhorAlternativa > qValorAtual * 1.1) // Exemplo de limiar
                {
                    individuo.SequenciaAcoes[i] = melhorAcaoAlternativa;
                    fitnessRecalculadoNecessario = true; // Marcar para recalcular fitness pois a sequência mudou
                    // Atualizar estadoCorrenteParaMutacao com base na nova ação se possível
                    var transicaoNova = interacoesEstagiario.FirstOrDefault(intr => intr.EstadoAtualId == estadoCorrenteParaMutacao && intr.AcaoTomadaId == melhorAcaoAlternativa);
                    if (transicaoNova != null) estadoCorrenteParaMutacao = transicaoNova.ProximoEstadoId;
                    else break; // Se a nova ação não tem transição conhecida, interrompe a mutação desta sequência
                }
                else
                {
                    // Avançar para o próximo estado com a ação original (ou já mutada)
                    var transicaoOriginal = interacoesEstagiario.FirstOrDefault(intr => intr.EstadoAtualId == estadoCorrenteParaMutacao && intr.AcaoTomadaId == individuo.SequenciaAcoes[i]);
                    if (transicaoOriginal != null) estadoCorrenteParaMutacao = transicaoOriginal.ProximoEstadoId;
                    else break; // Caminho quebrado, não pode continuar a mutar/avaliar
                }
            }
            // Se fitnessRecalculadoNecessario, o fitness do indivíduo deve ser atualizado.
            // O controller já recalcula após GerarNovaGeracao, mas aqui seria uma otimização
            // se a mutação for feita e o fitness atualizado dentro do GerarNovaGeracao.
            // A forma mais simples é o GerarNovaGeracao retornar os filhos e o controller/laço principal recalcular.
            // No entanto, a lógica no GerarNovaGeracaoDeterminista já chama CalcularFitnessParaNovaSequencia para os filhos.
            // Se a mutação ocorre *depois* desse cálculo, então um novo cálculo é necessário.
        }

    }
}

