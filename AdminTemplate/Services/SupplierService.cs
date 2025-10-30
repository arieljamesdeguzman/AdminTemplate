using System.Collections.Generic;
using System.Threading.Tasks;
using AdminTemplate.DTOs;
using AdminTemplate.Models;
using AdminTemplate.Repositories;

namespace AdminTemplate.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repository;

        public SupplierService(ISupplierRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync() =>
            await _repository.GetAllAsync();

        public async Task<Supplier> GetByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task AddAsync(SupplierDto dto)
        {
            var supplier = new Supplier
            {
                SupplierName = dto.SupplierName,
                ContactNumber = dto.ContactNumber,
                Email = dto.Email,
                Address = dto.Address
            };
            await _repository.AddAsync(supplier);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(SupplierDto dto)
        {
            var supplier = await _repository.GetByIdAsync(dto.Id);
            if (supplier == null) return;

            supplier.SupplierName = dto.SupplierName;
            supplier.ContactNumber = dto.ContactNumber;
            supplier.Email = dto.Email;
            supplier.Address = dto.Address;

            await _repository.UpdateAsync(supplier);
            await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
            await _repository.SaveChangesAsync();
        }
    }
}