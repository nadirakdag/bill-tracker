using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillController : ControllerBase
    {
        private readonly ILogger<BillController> _logger;

        public BillController(ILogger<BillController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return Ok();
        }

        [HttpPost]
        public IActionResult Post(object model)
        {
            return Created("", model);
        }

        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] object model)
        {
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            return NoContent();
        }
    }
}