using OnboardingAPI.Dto;
using OnboardingAPI.Models;

namespace OnboardingAPI.ServiceInterface
{
    public interface IAnaliseService
    {
        Task SalvarResultadoAsync(ResultadoAnaliseDto resultado);
        Task<ResultadoAnaliseDto> RodarAnaliseSemanalAsync(DateTime dataReferencia);
        
    }
}
