using OnboardingAPI.Models;

namespace OnboardingAPI.ServiceInterface
{
    public interface IAcaoService
    {
        IEnumerable<Acao> GetAll();
        Acao GetById(int id);
        void Create(Acao acao);
        void Update(int id, Acao acao);
        void Delete(int id);

    }
}
