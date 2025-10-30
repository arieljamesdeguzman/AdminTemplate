using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AdminTemplate.Services;
using AdminTemplate.DTOs;

namespace AdminTemplate.Controllers
{
    public class InventoryCategoryController : Controller
    {
        private readonly IInventoryCategoryService _service;
        public InventoryCategoryController(IInventoryCategoryService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _service.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(InventoryCategoryDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _service.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(InventoryCategoryDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _service.UpdateAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _service.GetByIdAsync(id);
            return View(category);
        }

        // Delete (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null)
            {
                TempData["ErrorMessage"] = "Failed to delete category.";
                return RedirectToAction(nameof(Index));
            }

            await _service.DeleteAsync(id);
            TempData["SuccessMessage"] = "Category deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
