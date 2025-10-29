using AdminTemplate.DTOs;
using AdminTemplate.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AdminTemplate.DTOs;
using AdminTemplate.Services;

namespace AdminTemplate.Controllers
{
    public class SignupController : Controller
    {
        private readonly IUserService _userService;

        public SignupController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Index()
        {
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
