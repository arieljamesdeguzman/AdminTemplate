using System.Collections.Generic;
using System.Threading.Tasks;
using AdminTemplate.Models;

namespace AdminTemplate.Repositories
{
    public interface IInventoryCategoryRepository
    {
        Task<IEnumerable<InventoryCategory>> GetAllAsync();
        Task<InventoryCategory> GetByIdAsync(int id);
        Task AddAsync(InventoryCategory category);
        Task UpdateAsync(InventoryCategory category);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}