using AdminTemplate.DTOs;
using AdminTemplate.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminTemplate.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory> GetByIdAsync(int id);
        Task AddAsync(InventoryDto dto);
        Task<bool> UpdateAsync(InventoryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}