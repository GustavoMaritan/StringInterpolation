using Microsoft.AspNetCore.Mvc;
using StringInterpolation.Core.Domain;
using StringReplacement.AspNetCore;

namespace ApiTests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "{{chave1}}", "Chilly", "Cool", "Mild", "Warm", "{{chave2}}", "Hot", "Sweltering", "{{chave -u}}"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{id}/{teste}")]
        [EnableTextReplacement]
        public IActionResult Get(
            [FromHeader] string partner = "item1",
            [FromQuery] string nome = "gustavo",
            [FromQuery] string nome2 = "gustavo asdas",
            [FromQuery] string nome3 = "gustavo ddddedd",
            [FromRoute] string id = "gm",
            [FromRoute] string teste = "valor para teste"
        )
        {
            return Ok(Summaries);
        }

        [HttpGet("{id}/{teste}/attr")]
        [EnableTextReplacement(SearchKey.Route, "id")]
        public IActionResult GetAttr(
           [FromHeader] string partner = "item1",
           [FromQuery] string nome = "gustavo",
           [FromQuery] string nome2 = "gustavo asdas",
           [FromQuery] string nome3 = "gustavo ddddedd",
           [FromRoute] string id = "gm",
           [FromRoute] string teste = "valor para teste"
        )
        {
            return Ok(Summaries);
        }
    }
}