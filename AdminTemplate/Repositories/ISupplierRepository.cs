using System.Collections.Generic;
using System.Threading.Tasks;
using AdminTemplate.Models;

namespace AdminTemplate.Repositories
{
    public interface ISupplierRepository
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<Supplier> GetByIdAsync(int id);
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(int id);
        Task SaveChangesAsync();
    }
}
