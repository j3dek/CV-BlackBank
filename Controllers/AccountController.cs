using BlackBank.Models;
using BlackBank.Models.Api;
using BlackBank.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlackBank.Controllers
{
    public class AccountController : Controller
    {
        private readonly DaneUzytkownikaDbContext _context;
        private readonly JwtServices _jwtServices;

        public AccountController(DaneUzytkownikaDbContext context, JwtServices jwtServices)
        {
            _context = context;
            _jwtServices = jwtServices;
        }

        // Endpoint logowania
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel model)
        {
            var user = await _context.DaneUzytkownikow
                .FirstOrDefaultAsync(u => u.email == model.email && u.password == model.password);
            
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = _jwtServices.GenerateToken(user.email);
            return Ok(new LoginResponseModel {Token = token});

        }
    }

}
