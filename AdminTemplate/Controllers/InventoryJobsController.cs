using AdminTemplate.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace AdminTemplate.Controllers
{
    public class InventoryJobsController : Controller
    {
        private readonly IRecurringJobManager _recurringJobManager;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public InventoryJobsController(
            IRecurringJobManager recurringJobManager,
            IBackgroundJobClient backgroundJobClient)
        {
            _recurringJobManager = recurringJobManager;
            _backgroundJobClient = backgroundJobClient;
        }

        // View for managing Hangfire jobs
        public IActionResult Index()
        {
            return View();
        }

        // Manually trigger inventory check immediately
        [HttpPost]
        public IActionResult TriggerInventoryCheck()
        {
            _backgroundJobClient.Enqueue<IInventoryMonitoringService>(
                service => service.CheckInventoryLevelsAsync());

            TempData["SuccessMessage"] = "Inventory check job has been queued and will run shortly.";
            return RedirectToAction("Index");
        }

        // Update recurring job schedule
        [HttpPost]
        public IActionResult UpdateSchedule(string cronExpression)
        {
            try
            {
                _recurringJobManager.AddOrUpdate<IInventoryMonitoringService>(
                    "check-inventory-levels",
                    service => service.CheckInventoryLevelsAsync(),
                    cronExpression);

                TempData["SuccessMessage"] = $"Schedule updated to: {cronExpression}";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Failed to update schedule: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        // Pause recurring job
        [HttpPost]
        public IActionResult PauseJob()
        {
            _recurringJobManager.RemoveIfExists("check-inventory-levels");
            TempData["SuccessMessage"] = "Inventory monitoring job has been paused.";
            return RedirectToAction("Index");
        }

        // Resume recurring job with default schedule
        [HttpPost]
        public IActionResult ResumeJob()
        {
            _recurringJobManager.AddOrUpdate<IInventoryMonitoringService>(
                "check-inventory-levels",
                service => service.CheckInventoryLevelsAsync(),
                "*/30 * * * *"); // Every 30 minutes

            TempData["SuccessMessage"] = "Inventory monitoring job has been resumed (every 30 minutes).";
            return RedirectToAction("Index");
        }
    }
}