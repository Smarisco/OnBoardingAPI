using Microsoft.AspNetCore.Mvc;
using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using OnboardingAPI.Services;

namespace OnboardingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstagiarioController: ControllerBase
    {
        private readonly IEstagiarioService _service;


        public EstagiarioController(IEstagiarioService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var estagiario = _service.GetById(id);
            if (estagiario == null)
                return NotFound();
            return Ok(estagiario);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Estagiario estagiario)
        {
            _service.Create(estagiario);
            return CreatedAtAction(nameof(GetById), new { id = estagiario.Id }, estagiario);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Estagiario estagiario)
        {
            var existing = _service.GetById(id);
            if (existing == null)
                return NotFound();

            estagiario.Id = id;
            _service.Update(id, estagiario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var existing = _service.GetById(id);
            if (existing == null)
                return NotFound();

            _service.Delete(id);
            return NoContent();
        }
    }
}
