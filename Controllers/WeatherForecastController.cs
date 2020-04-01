using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace dotnetCoreAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string Summaries = "Freezing";

        public WeatherForecastController()
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
          return Ok(Summaries);
        }
    }
}
