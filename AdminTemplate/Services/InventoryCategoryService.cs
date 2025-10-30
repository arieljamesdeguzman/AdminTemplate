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
        public InventoryCategoryService(IInventoryCategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<InventoryCategory>> GetAllAsync() =>
            await _repository.GetAllAsync();

        public async Task<InventoryCategory> GetByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task AddAsync(InventoryCategoryDto dto)
        {
            var category = new InventoryCategory { Name = dto.Name };
            await _repository.AddAsync(category);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(InventoryCategoryDto dto)
        {
            var category = await _repository.GetByIdAsync(dto.Id);
            if (category == null) return;
            category.Name = dto.Name;
            await _repository.UpdateAsync(category);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }
    }
}