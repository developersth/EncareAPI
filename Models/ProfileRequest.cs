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
        [BsonElement("Sex")]
        public string Sex { get; set; }
        [BsonElement("Phone")]
        public string? Phone { get; set; } = null!;
        [BsonElement("Birthday")]
        public DateTime? Birthday { get; set; }

    }
}