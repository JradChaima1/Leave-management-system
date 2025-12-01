using Leave.Core.Models;
using Leave.Core.Interfaces;
using Leave.Core.Exceptions;
using Leave.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Leave.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly LeaveContext _context;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IRepository<User> userRepository, LeaveContext context, ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _context = context;
            _logger = logger;
        }

        public async Task<User> AuthenticateAsync(string username, string password)
        {
            _logger.LogInformation("Login attempt for username: {Username}", username);
            
            var users = await _userRepository.GetAllAsync();
            var user = users.FirstOrDefault(u => u.Username == username && u.IsActive);

            if (user == null)
            {
                _logger.LogWarning("Login failed: User {Username} not found or inactive", username);
                return null;
            }

            if (!VerifyPassword(password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid password for user {Username}", username);
                return null;
            }

            _logger.LogInformation("Login successful: User {Username}, Role {RoleId}", username, user.RoleId);
            return user;
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            _logger.LogInformation("Registering new user: {Username}", user.Username);
            
            if (await UserExistsAsync(user.Username))
            {
                _logger.LogWarning("Registration failed: Username {Username} already exists", user.Username);
                throw new ValidationException("Username already exists");
            }

            user.PasswordHash = HashPassword(password);
            user.IsActive = true;
            user.CreatedDate = DateTime.Now;

            var result = await _userRepository.AddAsync(user);
            _logger.LogInformation("User registered successfully: {Username}, UserId {UserId}", user.Username, result.UserId);
            return result;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            var users = await _userRepository.GetAllAsync();
            return users.Any(u => u.Username == username);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            _logger.LogInformation("Fetching user with ID: {UserId}", userId);
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            _logger.LogInformation("Fetching all users");
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            _logger.LogInformation("Updating user: {UserId}", user.UserId);
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("User updated successfully: {UserId}", user.UserId);
        }

        public async Task ChangePasswordAsync(int userId, string newPassword)
        {
            _logger.LogInformation("Changing password for user: {UserId}", userId);
            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Password change failed: User {UserId} not found", userId);
                throw new NotFoundException("User not found");
            }

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation("Password changed successfully for user: {UserId}", userId);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string hash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hash);
            }
            catch
            {
                return false;
            }
        }
    }
}
