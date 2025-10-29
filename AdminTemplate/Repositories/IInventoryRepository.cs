using AdminTemplate.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminTemplate.Repositories
{
    public interface IInventoryRepository
    {
        Task<IEnumerable<Inventory>> GetAllAsync();
        Task<Inventory> GetByIdAsync(int id);
        Task AddAsync(Inventory inventory);
        Task UpdateAsync(Inventory inventory);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}