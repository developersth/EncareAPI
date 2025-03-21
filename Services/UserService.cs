// Data Access (Services/UserService.cs)
using EncareAPI.Models;
using MongoDB.Driver;
using System.Text;
using System.Security.Cryptography;

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
        public async Task<User> GetUserByEmail(string email) =>
           await _users.Find(user => user.Email == email).FirstOrDefaultAsync();
        public async Task<User> GetUserByGoogleIdAsync(string googleId) =>
            await _users.Find(x => x.GoogleId == googleId).FirstOrDefaultAsync();

        public async Task<User> CreateUserAsync(User user)
        {
            await _users.InsertOneAsync(user);
            return user; // Return the created user (important for getting the ID)
        }

        public async Task UpdateUserAsync(User updatedUser)
        {
            var result = await _users.ReplaceOneAsync(user => user.Email == updatedUser.Email, updatedUser);
            if (!result.IsAcknowledged)
            {
                throw new Exception("User update failed.");
            }

        }
        public async Task<bool> DeleteUserAsync(string email)
        {
            var result = await _users.DeleteOneAsync(user => user.Email == email);
            return result.DeletedCount > 0;
        }

        public async Task SetPasswordResetToken(string email, string token, DateTime expiration)
        {
            var update = Builders<User>.Update
                .Set(u => u.ResetToken, token)
                .Set(u => u.ResetTokenExpiration, expiration);

            await _users.UpdateOneAsync(u => u.Email == email, update);
        }

        public async Task<User> GetUserByResetTokenAsync(string token)
        {
            return await _users.Find(user => user.ResetToken == token && user.ResetTokenExpiration > DateTime.UtcNow).FirstOrDefaultAsync();
        }

        public async Task ResetPasswordAsync(string email, string newPassword)
        {
            var hashedPassword = HashPassword(newPassword);
            var update = Builders<User>.Update
                .Set(u => u.PasswordHash, hashedPassword)
                .Set(u => u.ResetToken, null) // Clear token after reset
                .Set(u => u.ResetTokenExpiration, null);

            await _users.UpdateOneAsync(u => u.Email == email, update);
        }

        public static string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }


        public async Task<User> GetUserByIdAsync(string id) =>
            await _users.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<User> GetUserByEmailAsync(string email) =>
            await _users.Find(x => x.Email == email).FirstOrDefaultAsync();
    }
}