using Microsoft.EntityFrameworkCore;
using Satur8.CoreApplication.Interfaces;
using Satur8.Domain.Models;
using Satur8.Persistence.Misc;
using System.Text.Json;

namespace Satur8.Persistence.Services
{
    public class PresetService
    {
        private readonly ISaturatorDbContext _dbContext;

        public PresetService(ISaturatorDbContext dbContext) => _dbContext = dbContext;

        public async Task<List<PresetDto>> GetAllAsync(Guid? userId)
        {
            var presets = await _dbContext.Presets.Include(p => p.Category)
                                                  .Include(p => p.Favourites)
                                                  .Include(p => p.User)
                                                  .ToListAsync();

            return presets.Select(p => new PresetDto
            {
                PresetId = p.PresetId,
                Name = p.Name ?? "",
                Description = p.Description ?? "",
                CategoryName = p.Category?.Name ?? "Без категории",
                AuthorLogin = p.User?.Login ?? "",
                IsFavourite = userId.HasValue && p.Favourites.Any(f => f.UserId == userId),
                Parameters = JsonSerializer.Deserialize<PresetParameters>(p.ParametersJson) ?? new PresetParameters()
            }).ToList();
        }

        public async Task<List<string>> GetCategoriesAsync()
        {
            return await _dbContext.Categories.Select(c => c.Name ?? "")
                                              .Where(n => n != "")
                                              .ToListAsync();
        }

        public async Task<ServiceResult> SavePresetAsync(Guid userId, string name, string description, string categoryName, 
            PresetParameters parameters, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return ServiceResult.Fail("Введите название пресета.");
            }

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                return ServiceResult.Fail("Введите категорию.");
            }

            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);

            if (category == null)
            {
                category = new Category
                {
                    CategoryId = Guid.NewGuid(),
                    Name = categoryName
                };
                _dbContext.Categories.Add(category);
            }

            var preset = new Preset
            {
                PresetId = Guid.NewGuid(),
                Name = name,
                Description = description,
                CategoryId = category.CategoryId,
                UserId = userId,
                ParametersJson = JsonSerializer.Serialize(parameters)
            };

            _dbContext.Presets.Add(preset);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return ServiceResult.Ok();
        }

        public async Task<ServiceResult> ToggleFavouriteAsync(Guid userId, Guid presetId, CancellationToken cancellationToken = default)
        {
            var fav = await _dbContext.Favourites.FirstOrDefaultAsync(f => f.PresetId == presetId && f.UserId == userId);

            if (fav == null)
            {
                _dbContext.Favourites.Add(new Favourite
                {
                    FavouritesId = Guid.NewGuid(),
                    UserId = userId,
                    PresetId = presetId
                });
            }
            else
            {
                _dbContext.Favourites.Remove(fav);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            return ServiceResult.Ok();
        }
    }
}
