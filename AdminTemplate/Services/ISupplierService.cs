using System.Collections.Generic;
using System.Threading.Tasks;
using AdminTemplate.DTOs;
using AdminTemplate.Models;

namespace AdminTemplate.Services
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<Supplier> GetByIdAsync(int id);
        Task AddAsync(SupplierDto dto);
        Task UpdateAsync(SupplierDto dto);
        Task DeleteAsync(int id);
    }
}