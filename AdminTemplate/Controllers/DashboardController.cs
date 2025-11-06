using AdminTemplate.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using AdminTemplate.Configuration;
using Microsoft.Extensions.Options;

namespace AdminTemplate.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly GoogleMapsSettings _googleMapsSettings;

        public DashboardController(IUserRepository userRepository, IOptions<GoogleMapsSettings> googleMapsSettings)
        {
            _userRepository = userRepository;
            _googleMapsSettings = googleMapsSettings.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.GoogleMapsApiKey = _googleMapsSettings.ApiKey;
            return View();
        }

        // ✅ NEW: API endpoint to get all users with coordinates
        [HttpGet]
        public async Task<IActionResult> GetUsersLocations()
        {
            var users = await _userRepository.GetAllUsersWithCoordinatesAsync();

            var userLocations = users
                .Where(u => !string.IsNullOrEmpty(u.Coordinates))
                .Select(u => new
                {
                    id = u.Id,
                    fullName = u.FullName,
                    email = u.Email,
                    address = u.Address,
                    coordinates = u.Coordinates
                })
                .ToList();

            return Json(userLocations);
        }
    }
}