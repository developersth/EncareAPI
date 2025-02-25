using EncareAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace EncareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : Controller
    {
        private readonly Random _random = new Random();
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public HomeController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet("health-metrics")]
        public IActionResult GetHealthMetrics()
        {
            var healthMetrics = new
            {
                HeartRate = _random.Next(60, 100), // BPM
                Steps = $"{_random.Next(500, 5000) / 1000.0:F1}k", // Steps in thousands
                O2Saturation = $"{_random.Next(92, 100)}%", // O2 Saturation in %
                Calories = _random.Next(20, 500), // kcal
                BloodSugar = _random.Next(70, 140), // mg/dl
                Sleep = $"{_random.Next(4, 9)} Hr {_random.Next(0, 60)} min" // Sleep duration
            };

            return Ok(healthMetrics);
        }
    }
}
