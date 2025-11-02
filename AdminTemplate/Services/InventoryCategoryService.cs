using System.Collections.Generic;
using System.Threading.Tasks;
using AdminTemplate.DTOs;
using AdminTemplate.Models;
using AdminTemplate.Repositories;

namespace AdminTemplate.Services
{
    public class InventoryCategoryService : IInventoryCategoryService
    {
        private readonly IInventoryCategoryRepository _repository;
        private readonly IFileService _fileService;

        public InventoryCategoryService(
            IInventoryCategoryRepository repository,
            IFileService fileService)
        {
            _repository = repository;
            _fileService = fileService;
        }

        public async Task<IEnumerable<InventoryCategory>> GetAllAsync() =>
            await _repository.GetAllAsync();

        public async Task<InventoryCategory> GetByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task AddAsync(InventoryCategoryDto dto)
        {
            var category = new InventoryCategory
            {
                Name = dto.Name
            };

            // Handle image upload
            if (dto.ImageFile != null)
            {
                category.ImageUrl = await _fileService.SaveFileAsync(dto.ImageFile, "categories");
            }

            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(InventoryCategoryDto dto)
        {
            var category = await _repository.GetByIdAsync(dto.Id);
            if (category == null) return;

            category.Name = dto.Name;

            // Handle image update
            if (dto.ImageFile != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(category.ImageUrl))
                {
                    await _fileService.DeleteFileAsync(category.ImageUrl);
                }

                // Save new image
                category.ImageUrl = await _fileService.SaveFileAsync(dto.ImageFile, "categories");
            }

            await _repository.UpdateAsync(category);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category != null && !string.IsNullOrEmpty(category.ImageUrl))
            {
                // Delete image file
                await _fileService.DeleteFileAsync(category.ImageUrl);
            }

            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }
    }
}