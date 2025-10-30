using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AdminTemplate.Data;
using AdminTemplate.Models;

namespace AdminTemplate.Repositories
{
    public class InventoryCategoryRepository : IInventoryCategoryRepository
    {
        private readonly ApplicationDbContext _context;
        public InventoryCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InventoryCategory>> GetAllAsync() =>
            await _context.InventoryCategories.ToListAsync();

        public async Task<InventoryCategory> GetByIdAsync(int id) =>
            await _context.InventoryCategories.FirstOrDefaultAsync(c => c.Id == id);

        public async Task AddAsync(InventoryCategory category)
        {
            await _context.InventoryCategories.AddAsync(category);
        }

        public async Task UpdateAsync(InventoryCategory category)
        {
            _context.InventoryCategories.Update(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await GetByIdAsync(id);
            if (category != null)
                _context.InventoryCategories.Remove(category);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}