using Microsoft.AspNetCore.Mvc;
using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using OnboardingAPI.Services;

namespace OnboardingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InteracaoController : ControllerBase
    {
        private readonly IInteracaoService _service;

        public InteracaoController(IInteracaoService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var interacao = _service.GetById(id);
            if (interacao == null) return NotFound();
            return Ok(interacao);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Interacao interacao)
        {
            _service.Create(interacao);
            return CreatedAtAction(nameof(GetById), new { id = interacao.Id }, interacao);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Interacao interacao)
        {
            var existing = _service.GetById(id);
            if (existing == null) return NotFound();

            interacao.Id = id;
            _service.Update(id, interacao);
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
