using AdminTemplate.DTOs;
using AdminTemplate.Models;
using AdminTemplate.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminTemplate.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;

        public InventoryService(IInventoryRepository inventoryRepository)
        {
            _inventoryRepository = inventoryRepository;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _inventoryRepository.GetAllAsync();
        }

        public async Task<Inventory> GetByIdAsync(int id)
        {
            return await _inventoryRepository.GetByIdAsync(id);
        }

        public async Task AddAsync(InventoryDto dto)
        {
            var item = new Inventory
            {
                ItemName = dto.ItemName,
                CategoryId = dto.CategoryId,
                SupplierId = dto.SupplierId,
                Unit = dto.Unit,
                CurrentQuantity = dto.CurrentQuantity,
                ReorderLevel = dto.ReorderLevel,
                CostPerUnit = dto.CostPerUnit,
                SellingPrice = dto.SellingPrice,
                Location = dto.Location,
                ExpiryDate = dto.ExpiryDate,
                Status = dto.Status ?? "active",
                DateAdded = DateTime.UtcNow,
                LastUpdated = DateTime.UtcNow
            };

            await _inventoryRepository.AddAsync(item);
            await _inventoryRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(InventoryDto dto)
        {
            var existing = await _inventoryRepository.GetByIdAsync(dto.Id);
            if (existing == null)
                return false;

            existing.ItemName = dto.ItemName;
            existing.CategoryId = dto.CategoryId;
            existing.SupplierId = dto.SupplierId;
            existing.Unit = dto.Unit;
            existing.CurrentQuantity = dto.CurrentQuantity;
            existing.ReorderLevel = dto.ReorderLevel;
            existing.CostPerUnit = dto.CostPerUnit;
            existing.SellingPrice = dto.SellingPrice;
            existing.Location = dto.Location;
            existing.ExpiryDate = dto.ExpiryDate;
            existing.Status = dto.Status;
            existing.LastUpdated = DateTime.UtcNow;

            await _inventoryRepository.UpdateAsync(existing);
            await _inventoryRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var item = await _inventoryRepository.GetByIdAsync(id);
            if (item == null)
                return false;

            await _inventoryRepository.DeleteAsync(id);
            await _inventoryRepository.SaveChangesAsync();

            return true;
        }
    }
}