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
            // Remove ImageFile from validation since it's optional
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors and try again.";
                return View(dto);
            }

            try
            {
                await _service.AddAsync(dto);
                TempData["SuccessMessage"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error: {ex.Message}";
                return View(dto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();

            var dto = new InventoryCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                ImageUrl = category.ImageUrl
            };

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(InventoryCategoryDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            await _service.UpdateAsync(dto);
            TempData["SuccessMessage"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var category = await _service.GetByIdAsync(id);
            return View(category);
        }

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