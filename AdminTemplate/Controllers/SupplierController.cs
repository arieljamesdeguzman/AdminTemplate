using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AdminTemplate.Services;
using AdminTemplate.DTOs;

namespace AdminTemplate.Controllers
{
    public class SupplierController : Controller
    {
        private readonly ISupplierService _service;
        public SupplierController(ISupplierService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var suppliers = await _service.GetAllAsync();
            return View(suppliers);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(SupplierDto dto)
        {
            if (!ModelState.IsValid) return View(dto);
            await _service.AddAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var supplier = await _service.GetByIdAsync(id);
            //if (supplier == null)
            //{
            //    TempData["ErrorMessage"] = "Supplier not found.";
            //    return RedirectToAction(nameof(Index));
            //}

            //// Map entity to DTO
            //var dto = new SupplierDto
            //{
            //    Id = supplier.Id,
            //    SupplierName = supplier.SupplierName,
            //    ContactNumber = supplier.ContactNumber,
            //    Email = supplier.Email,
            //    Address = supplier.Address
            //};

            //return View(dto);

            var supplier = await _service.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SupplierDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _service.UpdateAsync(dto);
            TempData["SuccessMessage"] = "Supplier updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _service.GetByIdAsync(id);
            return View(supplier);
        }

        // Delete (GET)
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _service.GetByIdAsync(id);
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "Supplier not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        // Delete (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _service.GetByIdAsync(id);
            if (supplier == null)
            {
                TempData["ErrorMessage"] = "Failed to delete supplier.";
                return RedirectToAction(nameof(Index));
            }

            await _service.DeleteAsync(id);
            TempData["SuccessMessage"] = "Supplier deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}