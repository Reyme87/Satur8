using Microsoft.EntityFrameworkCore;
using Satur8.Domain.Models;
using Satur8.Persistence;

namespace Satur8.Tests.Common
{
    public class SaturatorContextFactory
    {
        public static Guid UserAId = Guid.NewGuid();
        public static Guid UserBId = Guid.NewGuid();

        public static Guid CategoryToDeleteId = Guid.NewGuid();
        public static Guid CategoryToUpdateId = Guid.NewGuid();

        public static Guid PresetForDeleteId = Guid.NewGuid();
        public static Guid PresetForUpdateId = Guid.NewGuid();

        public static SaturatorDbContext Create()
        {
            var options = new DbContextOptionsBuilder<SaturatorDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new SaturatorDbContext(options);

            context.Database.EnsureCreated();
            context.Users.AddRange(
                new User
                {
                    UserId = UserAId,
                    Login = "UserA",
                    PasswordHash = "HashA"
                },
                new User
                {
                    UserId = UserBId,
                    Login = "UserB",
                    PasswordHash = "HashB"
                }
            );

            context.SaveChanges();

            context.Categories.AddRange(
                new Category
                {
                    CategoryId = Guid.Parse("62989338-5FD4-42D0-AA6C-F8D623BA2967"),
                    Name = "Category1"
                },
                new Category
                {
                    CategoryId = Guid.Parse("365B3872-90E3-4B87-BB20-A88636E80D37"),
                    Name = "Category2"
                },
                new Category
                {
                    CategoryId = CategoryToDeleteId,
                    Name = "DeleteCategory"
                },
                new Category
                {
                    CategoryId = CategoryToUpdateId,
                    Name = "UpdateCategory"
                });

            context.SaveChanges();

            context.Presets.AddRange(
                new Preset
                {
                    PresetId = Guid.Parse("EB876720-5950-47E7-A68E-1B749BC1DDC6"),
                    Name = "Preset1",
                    UserId = UserAId,
                    Description = "Description1",
                    CategoryId = context.Categories.First(c => c.Name == "Category1").CategoryId,
                    ParametersJson = "{}"
                },
                new Preset
                {
                    PresetId = PresetForDeleteId,
                    Name = "PresetToDelete",
                    UserId = UserAId,
                    Description = "Delete",
                    CategoryId = context.Categories.First(c => c.Name == "DeleteCategory").CategoryId,
                    ParametersJson = "{}"
                },
                new Preset
                {
                    PresetId = PresetForUpdateId,
                    Name = "PresetToUpdate",
                    UserId = UserBId,
                    Description = "Update",
                    CategoryId = context.Categories.First(c => c.Name == "UpdateCategory").CategoryId,
                    ParametersJson = "{}"
                });

            context.SaveChanges();

            return context;
        }

        public static void Destroy(SaturatorDbContext context)
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}
