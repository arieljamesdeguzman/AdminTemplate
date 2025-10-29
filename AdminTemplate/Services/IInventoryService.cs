using AdminTemplate.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdminTemplate.DTOs;

namespace AdminTemplate.Services
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory> GetByIdAsync(int id);
        Task AddAsync(InventoryDto dto);
        Task UpdateAsync(InventoryDto dto);
        Task DeleteAsync(int id);
    }
}