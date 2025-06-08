using OnboardingAPI.Models;

namespace OnboardingAPI.ServiceInterface
{
    /// <summary>
    /// Define o contrato para o serviço de Q-Learning.
    /// A principal responsabilidade é aprender uma política (Tabela Q) a partir de dados de interação.
    /// </summary>
    public interface IQLearningService
    {
        /// <summary>
        /// Executa o algoritmo Q-Learning com base em um histórico de interações
        /// e retorna a política aprendida na forma de uma Tabela Q.
        /// </summary>
        /// <param name="interacoes">A lista de interações observadas para treinar o algoritmo.</param>
        /// <returns>
        /// Uma Task contendo um Dicionário que representa a Tabela Q.
        /// A chave do dicionário é um Tuple (estadoId, acaoId) e o valor é o double correspondente ao valor Q.
        /// </returns>
        Task<Dictionary<Tuple<int, int>, double>> ExecutarEObterTabelaQ(List<Interacao> interacoes);



    }
}
