using System.ComponentModel.DataAnnotations;

namespace EncareAPI.Models
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Name { get; set; } = null!;

    }
}