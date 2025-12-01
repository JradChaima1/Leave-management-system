using Leave.Core.Models;

namespace Leave.Core.Interfaces
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> RegisterAsync(User user, string password);
        Task<bool> UserExistsAsync(string username);
        Task<User> GetUserByIdAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task UpdateUserAsync(User user);
        Task ChangePasswordAsync(int userId, string newPassword);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
    }
}
