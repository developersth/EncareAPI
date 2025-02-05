using MongoDB.Bson.Serialization.Attributes;
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

        [Required]
        public string Password { get; set; } // Store hashed passwords
        public string? Phone { get; set; } = null!;
        public DateTime? Birthday { get; set; }

    }
}