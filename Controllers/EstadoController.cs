using Microsoft.AspNetCore.Mvc;
using OnboardingAPI.Models;
using OnboardingAPI.ServiceInterface;
using OnboardingAPI.Services;

namespace OnboardingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstadoController : ControllerBase
    {
        private readonly IEstadoService _service;

        public EstadoController(IEstadoService service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult GetAll() => Ok(_service.GetAll());

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var estado = _service.GetById(id);
            if (estado == null) return NotFound();
            return Ok(estado);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Estado estado)
        {
            _service.Create(estado);
            return CreatedAtAction(nameof(GetById), new { id = estado.Id }, estado);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Estado estado)
        {
            var existing = _service.GetById(id);
            if (existing == null) return NotFound();

            estado.Id = id;
            _service.Update(id, estado);
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
