using OnboardingAPI.Models;

namespace OnboardingAPI.ServiceInterface
{
    public interface IInteracaoService
    {
        IEnumerable<Interacao> GetAll();
        Interacao GetById(int id);
        void Create(Interacao interacao);
        void Update(int id, Interacao interacao);
        void Delete(int id);

        Task<IEnumerable<Interacao>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim);
    }
}
