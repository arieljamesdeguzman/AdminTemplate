using System.Collections.Generic;
using System.Threading.Tasks;
using AdminTemplate.DTOs;
using AdminTemplate.Models;

namespace AdminTemplate.Services
{
    public interface IInventoryCategoryService
    {
        Task<IEnumerable<InventoryCategory>> GetAllAsync();
        Task<InventoryCategory> GetByIdAsync(int id);
        Task AddAsync(InventoryCategoryDto dto);
        Task UpdateAsync(InventoryCategoryDto dto);
        Task DeleteAsync(int id);
    }
}