using AdminTemplate.DTOs;
using AdminTemplate.Services;
using AdminTemplate.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace AdminTemplate.Controllers
{
    public class SignupController : Controller
    {
        private readonly IUserService _userService;
        private readonly GoogleMapsSettings _googleMapsSettings;

        // ✅ Inject GoogleMapsSettings
        public SignupController(IUserService userService, IOptions<GoogleMapsSettings> googleMapsSettings)
        {
            _userService = userService;
            _googleMapsSettings = googleMapsSettings.Value;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // ✅ Pass API Key to View via ViewBag
            ViewBag.GoogleMapsApiKey = _googleMapsSettings.ApiKey;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] SignupDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _userService.RegisterAsync(dto);
            if (!success)
                return Conflict(new { message = "Email already exists." });

            return Ok(new { message = "Registration successful!" });
        }
    }
}