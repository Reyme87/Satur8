using Satur8.Domain.Models;

namespace Satur8.Persistence
{
    public class AuthResult
    {
        public bool Success { get; private set; }
        public string? Error { get; private set; }
        public User? User { get; private set; }

        public static AuthResult Ok(User user) => new() { Success = true, User = user };
        public static AuthResult Fail(string err) => new() { Success = false, Error = err };
    }
}
