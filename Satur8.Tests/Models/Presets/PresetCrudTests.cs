using Microsoft.EntityFrameworkCore;
using Satur8.Domain.Models;
using Satur8.Tests.Common;

namespace Satur8.Tests.Models.Presets
{
    public class PresetCrudTests : TestCrudBase
    {
        [Fact]
        public async Task CreatePreset_Success()
        {
            // Arrange
            var preset = new Preset
            {
                PresetId = Guid.NewGuid(),
                Name = "PresetName",
                UserId = SaturatorContextFactory.UserAId,
                Description = "Description",
                CategoryId = Guid.Parse("62989338-5FD4-42D0-AA6C-F8D623BA2967"),
                ParametersJson = "{}"
            };

            // Act
            await Context.Presets.AddAsync(preset);
            await Context.SaveChangesAsync();

            // Assert
            var created = await Context.Presets.FirstOrDefaultAsync(p => p.PresetId == preset.PresetId);
            Assert.NotNull(created);
            Assert.Equal(created.PresetId, preset.PresetId);
        }

        [Fact]
        public async Task ReadPreset_ReturnCorrectData()
        {
            // Arrange
            // Act
            var preset = await Context.Presets.FindAsync(SaturatorContextFactory.PresetForDeleteId);

            // Assert
            Assert.NotNull(preset);
            Assert.Equal("PresetToDelete", preset.Name);
        }

        [Fact]
        public async Task UpdatePreset_UpdateSuccessfully()
        {
            // Arrange
            var preset = await Context.Presets.FindAsync(SaturatorContextFactory.PresetForUpdateId);
            string name = "NewName";

            // Act
            preset.Name = name;
            await Context.SaveChangesAsync();

            // Assert
            var updated = await Context.Presets.FirstOrDefaultAsync(p => p.PresetId == SaturatorContextFactory.PresetForUpdateId);
            Assert.Equal(name, updated.Name);
        }

        [Fact]
        public async Task DeletePreset_DeleteSuccessfully()
        {
            // Arrange
            var preset = await Context.Presets.FindAsync(SaturatorContextFactory.PresetForDeleteId);

            // Act
            Context.Presets.Remove(preset!);
            await Context.SaveChangesAsync();

            // Assert
            var deleted = await Context.Presets.FirstOrDefaultAsync(p => p.PresetId == SaturatorContextFactory.PresetForDeleteId);
            Assert.Null(deleted);
        }
    }
}
