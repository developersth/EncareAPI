// Models/MongoDBSettings.cs (or in the same file as your other models)
namespace EncareAPI.Models
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = null!; // Required
        public string DatabaseName { get; set; } = null!; // Required
        public string UsersCollectionName { get; set; } = "users"; // Optional: Customize collection name
    }
}