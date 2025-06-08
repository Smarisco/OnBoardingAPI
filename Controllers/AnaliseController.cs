using Microsoft.AspNetCore.Mvc;
using OnboardingAPI.Dto;
using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using OnboardingAPI.Services;
using System.Globalization;

namespace OnboardingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] 
    public class AnaliseController : ControllerBase
    {
        private readonly IAnaliseService _analiseService;

        public AnaliseController(IAnaliseService analiseService)
        {
            _analiseService = analiseService ?? throw new ArgumentNullException(nameof(analiseService));
        }

        [HttpPost("rodar-analise-semanal")]
        [ProducesResponseType(typeof(ResultadoAnaliseDto), 200)]
        [ProducesResponseType(typeof(string), 500)]
        public async Task<IActionResult> RodarAnaliseCompleta([FromQuery] DateTime? dataReferencia)
        {
            try
            {
                
                var resultado = await _analiseService.RodarAnaliseSemanalAsync(dataReferencia ?? DateTime.Now);

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                
                Console.Error.WriteLine($"Erro crítico ao executar a análise semanal: {ex.Message}");
                return StatusCode(500, $"Ocorreu um erro interno no servidor ao processar a análise: {ex.Message}");
            }
        }

    }

}
