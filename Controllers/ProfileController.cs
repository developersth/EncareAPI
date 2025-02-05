using System.Globalization;
using EncareAPI.Models;
using EncareAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Linq;


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
        [Authorize]
        public async Task<IActionResult> GetProfile([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var newUser = new UserInfo
            {
                Email = user.Email,
                FullName = user.Name,
                Gender = user.Gender,
                DateOfBirth = user.Birthday.Value.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US")),
                Age = CalculateAge(user.Birthday)

            };
            return Ok(newUser);
        }

        [HttpPut("edit")]
        [Authorize]
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
                Id = existingUser.Id,
                GoogleId = existingUser.GoogleId,
                Email = profileRequest.Email,
                Name = profileRequest.Name,
                PasswordHash = UserService.HashPassword(profileRequest.Password),
                Gender = profileRequest.Gender,
                Birthday = profileRequest.Birthday,
                Phone = profileRequest.Phone,
                Height = profileRequest.Height,
                Weight = profileRequest.Weight
            };

            await _userService.UpdateUserAsync(newUser); // Get the user with the generated ID

            return Ok(new { message = "Profile updated successfully" });
        }

        [HttpGet("settings")]
        [Authorize]
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
             [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { message = "User logged out successfully" });
        }
        // 📌 ฟังก์ชันคำนวณอายุจากวันเกิด
        private int CalculateAge(DateTime? birthday)
        {
            if (birthday == null) return 0; // ถ้าไม่มีวันเกิด ให้คืนค่า 0

            var today = DateTime.Today;
            var age = today.Year - birthday.Value.Year;

            // ตรวจสอบว่าวันเกิดปีนี้มาถึงหรือยัง (ถ้ายังอายุยังไม่ถึง ต้อง -1)
            if (birthday.Value.Date > today.AddYears(-age))
            {
                age--;
            }

            return age;
        }
    }
}
