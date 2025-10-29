using AdminTemplate.Data;
using AdminTemplate.Models;
using AdminTemplate.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminTemplate.Repositories
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly ApplicationDbContext _context;

        public InventoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Inventory>> GetAllAsync()
        {
            return await _context.Inventories
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .ToListAsync();
        }

        public async Task<Inventory> GetByIdAsync(int id)
        {
            return await _context.Inventories
                .Include(i => i.Category)
                .Include(i => i.Supplier)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task AddAsync(Inventory inventory)
        {
            await _context.Inventories.AddAsync(inventory);
        }

        public async Task UpdateAsync(Inventory inventory)
        {
            _context.Inventories.Update(inventory);
        }

        public async Task DeleteAsync(int id)
        {
            var inventory = await GetByIdAsync(id);
            if (inventory != null)
                _context.Inventories.Remove(inventory);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
