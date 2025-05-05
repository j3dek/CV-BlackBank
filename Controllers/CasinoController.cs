using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BlackBank.Models;
// using BlackBank.Data;

namespace BlackBank.Controllers
{
    public class CasinoController : Controller
    {
        private readonly DaneUzytkownikaDbContext _context;

        public CasinoController(DaneUzytkownikaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Crash()
        {
            // Pobierz saldo użytkownika, jeśli jest zalogowany
            if (User.Identity.IsAuthenticated)
            {
                var userId = HttpContext.Session.GetInt32("UserId");
                if (userId != null)
                {
                    var user = await _context.DaneUzytkownikow.FindAsync(userId);
                    if (user != null)
                    {
                        ViewBag.Balance = user.balance;
                    }
                }
            }
            
            return View();
        }

        public IActionResult BlackJack()
        {
            return View();
        }

        public IActionResult Slots()
        {
            return View();
        }
    }
}