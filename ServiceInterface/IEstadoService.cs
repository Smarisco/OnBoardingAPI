using OnboardingAPI.Models;

namespace OnboardingAPI.ServiceInterface
{
    public interface IEstadoService
    {
        IEnumerable<Estado> GetAll();
        Estado GetById(int id);
        void Create(Estado estado);
        void Update(int id, Estado estado);
        void Delete(int id);
        
    }
}
