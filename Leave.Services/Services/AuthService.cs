using Leave.Core.Models;
using Leave.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Leave.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;

        public AuthService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Username == username && u.IsActive);

            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            if (await UserExistsAsync(user.Username))
                throw new Exception("Username already exists");

            user.PasswordHash = HashPassword(password);
            user.IsActive = true;
            user.CreatedDate = DateTime.Now;

            return await _userRepository.AddAsync(user);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Any(u => u.Username == username);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
        }

        public string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public bool VerifyPassword(string password, string hash)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hash;
        }
    }
}
