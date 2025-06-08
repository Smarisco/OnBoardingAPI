using OnboardingAPI.Models;

namespace OnboardingAPI.ServiceInterface
{
    public interface IEstagiarioService
    {
        IEnumerable<Estagiario> GetAll();
        Estagiario GetById(int id);
        void Create(Estagiario estagiario);
        void Update(int id, Estagiario estagiario);
        void Delete(int id);
    }
}
