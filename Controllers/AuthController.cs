
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EncareAPI.Models;
using EncareAPI.Services;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace EncareAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;

        public AuthController(UserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")] // New registration endpoint
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest) // Use a DTO
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors
            }

            // Check if email is already registered
            var existingUser = await _userService.GetUserByEmailAsync(registerRequest.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email already exists." }); // 409 Conflict
            }

            var newUser = new User
            {
                Email = registerRequest.Email,
                Name = registerRequest.Name,
                PasswordHash = UserService.HashPassword(registerRequest.Password)
            };

            newUser = await _userService.CreateUserAsync(newUser); // Get the user with the generated ID

            // JWT generation (optional but recommended)
            var token = GenerateJwtToken(newUser);
            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, new { Token = token, User = newUser }); // 201 Created with location header and user data
        }


        [HttpGet("{id}", Name = "GetUser")] // Get user by ID (protected endpoint example)
        [Authorize]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userService.GetUserByEmailAsync(request.Email);
            if (user == null) return NotFound(new { message = "User not found." });

            // Generate reset token
            var resetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var expiration = DateTime.UtcNow.AddHours(1);
            await _userService.SetPasswordResetToken(request.Email, resetToken, expiration);

            // Generate reset link
            string resetUrl = $"https://yourdomain.com/reset-password?token={resetToken}";

            // Send email
            var emailService = new EmailService(_configuration);
            await emailService.SendEmailAsync(user.Email, "Password Reset Request",
                $"<p>Click <a href='{resetUrl}'>here</a> to reset your password.</p>");

            return Ok(new { message = "Password reset link sent to email." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var user = await _userService.GetUserByResetTokenAsync(request.Token);
            if (user == null)
            {
                return BadRequest(new { message = "Invalid or expired token." });
            }

            await _userService.ResetPasswordAsync(user.Email, request.NewPassword);
            return Ok(new { message = "Password has been reset successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin login)
        {
            var existingUser = await _userService.GetUserByEmail(login.Email);
            if (existingUser == null || existingUser.PasswordHash != UserService.HashPassword(login.Password))
                return Unauthorized(new { message = "Invalid credentials" });
            existingUser.PasswordHash = null;
            var token = GenerateJwtToken(existingUser);
            return Ok(new { Token = token, User = existingUser });
        }
        [HttpGet("google/login")]
        public IActionResult GoogleLogin()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse", "Auth", null, Request.Scheme) // Callback URL
            };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google/response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result.Succeeded)
            {
                var userClaims = result.Principal?.Identities.FirstOrDefault()?.Claims.ToList();

                if (userClaims != null)
                {
                    string googleId = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
                    string email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "";
                    string name = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "";

                    var existingUser = await _userService.GetUserByGoogleIdAsync(googleId);
                    var emailUser = await _userService.GetUserByEmailAsync(email);

                    if (existingUser == null)
                    {
                        if (emailUser != null)
                        {
                            // ❌ ป้องกันการลงทะเบียนซ้ำ
                            return BadRequest("This email is already registered. Please log in instead.");
                        }

                        // ✅ สร้างบัญชีใหม่เฉพาะกรณีที่อีเมลไม่มีในระบบ
                        var newUser = new User { GoogleId = googleId, Email = email, Name = name };
                        await _userService.CreateUserAsync(newUser);
                        existingUser = newUser;
                    }

                    // JWT Generation
                    var token = GenerateJwtToken(existingUser);

                    return Ok(new { Token = token });
                }
            }

            return BadRequest("Authentication failed.");
        }



        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])); // Get key from config
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Id), // Use MongoDB ID
                new Claim(ClaimTypes.Email, user.Email),
                // Add other claims as needed
            };

            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"], // From config
                _configuration["Jwt:Audience"], // From config
                claims,
                expires: DateTime.Now.AddDays(7), // Example expiration
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Example protected endpoint (using JWT)
        [Authorize]  // Requires JWT authentication
        [HttpGet("protected")]
        public IActionResult ProtectedResource()
        {
            return Ok("This is a protected resource.");
        }
    }
}