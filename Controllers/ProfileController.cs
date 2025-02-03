using EncareAPI.Models;
using EncareAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace EncareAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public ProfileController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> EditProfile([FromBody] ProfileRequest profileRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Check if email is already registered
            var existingUser = await _userService.GetUserByEmailAsync(profileRequest.Email);
            if (existingUser == null)
            {
                return Conflict(new { message = "User not found" }); // 409 Conflict
            }

            var newUser = new User
            {
                Name = profileRequest.Name,
                PasswordHash = UserService.HashPassword(profileRequest.Password),
                Sex = profileRequest.Sex,
                Birthday = profileRequest.Birthday,
                Phone =profileRequest.Phone,
            };

            newUser = await _userService.EditUserAsync(newUser); // Get the user with the generated ID



            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpGet("settings")]
        public IActionResult GetSettings()
        {
            return Ok(new { message = "User settings" });
        }

        [HttpGet("privacy-policy")]
        public IActionResult GetPrivacyPolicy()
        {
            return Ok(new { message = "Privacy Policy" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "User logged out successfully" });
        }
    }
}
