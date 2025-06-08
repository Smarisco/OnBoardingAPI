using OnboardingAPI.Models;

namespace OnboardingAPI.ServiceInterface
{
    
    public interface IAlgoritmoGeneticoService
    {
        
        Task<AnaliseAlgoritmoGenetico> Executar(List<Interacao> interacoes);
        
        Task<IEnumerable<IndividuoAG>> ObterPopulacaoInicialObservadaAsync(IEnumerable<Interacao> interacoes);
        IEnumerable<IndividuoAG> GerarNovaGeracaoDeterministaAsync(IEnumerable<IndividuoAG> populacao, IEnumerable<Interacao> interacoes, Dictionary<Tuple<int, int>, double> qTable);
        Task SalvarAnaliseSemanalAsync(AnaliseAlgoritmoGenetico analise);
    }
}
