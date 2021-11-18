using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RunnerController : ControllerBase
    {
        private readonly ILogger<RunnerController> _logger;
        private readonly IThirdSoftwareService _thirdSoftwareService;

        public RunnerController(ILogger<RunnerController> logger, IThirdSoftwareService thirdSoftwareService)
        {
            _logger = logger;
            _thirdSoftwareService = thirdSoftwareService;
        }

        [HttpGet("task1")]
        [HttpPost("task1")]
        public async Task<IActionResult> RunTask1()
        {
            var response = await _thirdSoftwareService.GreetService();

            return Ok(response);
        }
    }
}
