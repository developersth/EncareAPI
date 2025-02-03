using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EncareAPI.Models
{
    public class UserLogin
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } // Store hashed passwords

    }
}