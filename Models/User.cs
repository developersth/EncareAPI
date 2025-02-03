using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EncareAPI.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!; // Auto-generated by MongoDB

        public string GoogleId { get; set; } = null!; // Unique Google ID
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        // Add other properties as needed (e.g., Profile Picture, etc.)
    }
}