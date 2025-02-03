// Data Access (Services/UserService.cs)
using EncareAPI.Models;
using MongoDB.Driver;

namespace EncareAPI.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetValue<string>("MongoDB:ConnectionString")); // Get connection string from config
            var database = client.GetDatabase(configuration.GetValue<string>("MongoDB:DatabaseName")); // Get DB name from config
            _users = database.GetCollection<User>("users"); // Collection name
        }

        public async Task<User> GetUserByGoogleIdAsync(string googleId) =>
            await _users.Find(x => x.GoogleId == googleId).FirstOrDefaultAsync();

     public async Task<User> CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user; // Return the created user (important for getting the ID)
        }



        public async Task<User> GetUserByIdAsync(string id) =>
            await _users.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<User> GetUserByEmailAsync(string email) =>
            await _users.Find(x => x.Email == email).FirstOrDefaultAsync();
    }
}