using Microsoft.EntityFrameworkCore;
using Satur8.CoreApplication.Interfaces;
using Satur8.Domain.Models;
using System.Security.Cryptography;
using System.Text;

namespace Satur8.Persistence.Services
{
    public class AuthService
    {
        private readonly ISaturatorDbContext _dbContext;

        public AuthService(ISaturatorDbContext dbContext) => _dbContext = dbContext;

        public async Task<AuthResult> LoginAsync(string login, string password)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
            {
                return AuthResult.Fail("Пользователь не найден. Необходимо пройти регистрацию.");
            }

            if (user.PasswordHash != HashPassword(password))
            {
                return AuthResult.Fail("Неверный логин или пароль.");
            }

            return AuthResult.Ok(user);
        }

        public async Task<AuthResult> RegisterAsync(string login, string password, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(login) || login.Length < 3)
            {
                return AuthResult.Fail("Логин должен содержать не менее 3 символов.");
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                return AuthResult.Fail("Пароль должен содержать не менее 6 символов.");
            }

            var exists = await _dbContext.Users.AnyAsync(u => u.Login == login);

            if (exists)
            {
                return AuthResult.Fail("Пользователь с таким логином уже существует.");
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Login = login,
                PasswordHash = HashPassword(password)
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return AuthResult.Ok(user);
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes).ToLower();
        }
    }
}
