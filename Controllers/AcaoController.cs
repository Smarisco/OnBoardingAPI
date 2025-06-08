using Microsoft.AspNetCore.Mvc;
using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using OnboardingAPI.Services;

namespace OnboardingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcaoController : ControllerBase
    {
        private readonly IAcaoService _service;

        public AcaoController(IAcaoService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var acao = _service.GetById(id);
            if (acao == null) return NotFound();
            return Ok(acao);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Acao acao)
        {
            _service.Create(acao);
            return CreatedAtAction(nameof(GetById), new { id = acao.Id }, acao);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Acao acao)
        {
            var existing = _service.GetById(id);
            if (existing == null) return NotFound();

            acao.Id = id;
            _service.Update(id, acao);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _service.GetById(id);
            if (existing == null) return NotFound();

            _service.Delete(id);
            return NoContent();
        }
    }
}
