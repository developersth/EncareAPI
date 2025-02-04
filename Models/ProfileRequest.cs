using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace EncareAPI.Models
{
    public class ProfileRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string Password { get; set; } // Store hashed passwords
        public string Name { get; set; } = null!;
        public string Sex { get; set; }
        public string? Phone { get; set; } = null!;
        public DateTime? Birthday { get; set; }
        public double? Weight { get; set; } // น้ำหนัก (kg)
        public double? Height { get; set; } // ส่วนสูง (cm)

    }
}