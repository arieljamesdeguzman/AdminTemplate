using AdminTemplate.Data;
using AdminTemplate.DTOs;
using AdminTemplate.Services;
using AdminTemplate.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AdminTemplate.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IInventoryService _inventoryService;
        private readonly ApplicationDbContext _context;

        public InventoryController(IInventoryService inventoryService, ApplicationDbContext context)
        {
            _inventoryService = inventoryService;
            _context = context;
        }

        // Display all
        public async Task<IActionResult> Index()
        {
            var data = await _inventoryService.GetAllAsync();
            return View(data);
        }

        // Create (GET)
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = new InventoryViewModel
            {
                Categories = await _context.InventoryCategories.ToListAsync(),
                Suppliers = await _context.Suppliers.ToListAsync()
            };
            return View(vm);
        }

        // Create (POST)
        [HttpPost]
        public async Task<IActionResult> Create(InventoryDto dto)
        {
            if (!ModelState.IsValid)
                return View();

            await _inventoryService.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // Edit (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null) return NotFound();

            var vm = new InventoryViewModel
            {
                Inventory = item,
                Categories = await _context.InventoryCategories.ToListAsync(),
                Suppliers = await _context.Suppliers.ToListAsync()
            };
            return View(vm);
        }

        // Edit (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(InventoryDto dto)
        {
            if (!ModelState.IsValid)
                return View();

            await _inventoryService.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _inventoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Details
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            return View(item);
        }
    }
}
