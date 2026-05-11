using Microsoft.EntityFrameworkCore;
using Satur8.Domain.Models;
using Satur8.Tests.Common;

namespace Satur8.Tests.Models.Users
{
    public class UserCrudTests : TestCrudBase
    {
        [Fact]
        public async Task CreateUser_Success()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Login = "Login",
                PasswordHash = "Hash"
            };

            // Act
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync();

            // Assert
            var created = await Context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            Assert.NotNull(created);
        }

        [Fact]
        public async Task ReadUser_ReturnCorrectData()
        {
            // Arrange
            // Act
            var user = await Context.Users.FindAsync(SaturatorContextFactory.UserAId);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(user.Login, "UserA");
        }

        [Fact]
        public async Task UpdateUser_UpdateSuccessfully()
        {
            // Arrange
            var user = await Context.Users.FindAsync(SaturatorContextFactory.UserAId);
            var login = "NewLogin";

            // Act
            user.Login = login;
            await Context.SaveChangesAsync();

            // Assert
            var updated = await Context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            Assert.Equal(updated.Login, login);
        }

        [Fact]
        public async Task DeleteUser_DeleteSuccessfully()
        {
            // Arrange
            var user = await Context.Users.FindAsync(SaturatorContextFactory.UserBId);

            // Act
            Context.Users.Remove(user);
            await Context.SaveChangesAsync();

            // Assert
            var deleted = await Context.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
            Assert.Null(deleted);
        }
    }
}
