using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly ILogger<InfoController> _logger;

        public InfoController(ILogger<InfoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("Getting API Information");
            return Ok(new
            {
                Application = "bill-tracker",
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            });
        }
    }
}