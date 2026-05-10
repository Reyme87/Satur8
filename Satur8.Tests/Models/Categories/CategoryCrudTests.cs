using Microsoft.EntityFrameworkCore;
using Satur8.Domain.Models;
using Satur8.Tests.Common;

namespace Satur8.Tests.Models.Categories
{
    public class CategoryCrudTests : TestCrudBase
    {
        [Fact]
        public async Task CreateCategory_Success()
        {
            // Arrange
            var category = new Category
            {
                CategoryId = Guid.NewGuid(),
                Name = "NewCategory"
            };

            // Act
            await Context.Categories.AddAsync(category);
            await Context.SaveChangesAsync();

            // Assert
            var saved = await Context.Categories.FirstOrDefaultAsync(c => c.Name == "NewCategory");
            Assert.NotNull(saved);
            Assert.Equal(category.CategoryId, saved.CategoryId);
        }

        [Fact]
        public async Task ReadCategory_ReturnCorrectData()
        {
            // Arrange
            // Act
            var category = await Context.Categories.FindAsync(SaturatorContextFactory.CategoryToDeleteId);

            // Assert
            Assert.NotNull(category);
            Assert.Equal("DeleteCategory", category.Name);
        }

        [Fact]
        public async Task UpdateCategory_UpdateSuccessfully()
        {
            // Arrange
            var category = await Context.Categories.FindAsync(SaturatorContextFactory.CategoryToUpdateId);
            var name = "NewName";

            // Act
            category.Name = name;
            await Context.SaveChangesAsync();

            // Assert
            var updated = await Context.Categories.FirstOrDefaultAsync(c => c.CategoryId == SaturatorContextFactory.CategoryToUpdateId);
            Assert.Equal(name, updated.Name);
        }

        [Fact]
        public async Task DeleteCategory_DeleteSuccessfully()
        {
            // Arrange
            var category = await Context.Categories.FirstOrDefaultAsync(c => c.CategoryId == SaturatorContextFactory.CategoryToDeleteId);

            // Act
            Context.Categories.Remove(category!);
            await Context.SaveChangesAsync();

            // Assert
            var deleted = await Context.Categories.FindAsync(SaturatorContextFactory.CategoryToDeleteId);
            Assert.Null(deleted);
        }
    }
}
