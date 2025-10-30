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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(InventoryViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns on validation error
                vm.Categories = await _context.InventoryCategories.ToListAsync();
                vm.Suppliers = await _context.Suppliers.ToListAsync();
                return View(vm);
            }

            await _inventoryService.AddAsync(vm.Inventory);
            TempData["SuccessMessage"] = "Inventory item created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Edit (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Inventory item not found.";
                return RedirectToAction(nameof(Index));
            }

            // Map entity to DTO
            var dto = new InventoryDto
            {
                Id = item.Id,
                ItemName = item.ItemName,
                CategoryId = item.CategoryId,
                SupplierId = item.SupplierId,
                Unit = item.Unit,
                CurrentQuantity = item.CurrentQuantity,
                ReorderLevel = item.ReorderLevel,
                CostPerUnit = item.CostPerUnit,
                SellingPrice = item.SellingPrice,
                Location = item.Location,
                ExpiryDate = item.ExpiryDate,
                Status = item.Status
            };

            var vm = new InventoryViewModel
            {
                Inventory = dto,
                Categories = await _context.InventoryCategories.ToListAsync(),
                Suppliers = await _context.Suppliers.ToListAsync()
            };

            return View(vm);
        }

        // Edit (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(InventoryViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Reload dropdowns on validation error
                vm.Categories = await _context.InventoryCategories.ToListAsync();
                vm.Suppliers = await _context.Suppliers.ToListAsync();
                return View(vm);
            }

            var result = await _inventoryService.UpdateAsync(vm.Inventory);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to update inventory item.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Inventory item updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Inventory item not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _inventoryService.DeleteAsync(id);
            if (!result)
            {
                TempData["ErrorMessage"] = "Failed to delete inventory item.";
                return RedirectToAction(nameof(Index));
            }
            TempData["SuccessMessage"] = "Inventory item deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        // Details (GET)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _inventoryService.GetByIdAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Inventory item not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(item);
        }
    }
}