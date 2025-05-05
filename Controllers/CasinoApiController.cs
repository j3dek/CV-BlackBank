using BlackBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BlackBank.Controllers
{
    [Route("api/casino")]
    [ApiController]
    [Authorize]
    public class CasinoApiController : ControllerBase
    {
        private readonly DaneUzytkownikaDbContext _context;

        public CasinoApiController(DaneUzytkownikaDbContext context)
        {
            _context = context;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Użytkownik niezalogowany" });
            }

            var user = await _context.DaneUzytkownikow.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Nie znaleziono użytkownika" });
            }

            return Ok(new { success = true, balance = user.balance });
        }

        [HttpPost("placeBet")]
        public async Task<IActionResult> PlaceBet([FromBody] BetModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Użytkownik niezalogowany" });
            }

            var user = await _context.DaneUzytkownikow.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Nie znaleziono użytkownika" });
            }

            // Sprawdź czy użytkownik ma wystarczające środki
            if (user.balance < model.BetAmount)
            {
                return BadRequest(new { success = false, message = "Niewystarczające środki" });
            }

            try
            {
                // Odejmij stawkę z konta gracza
                user.balance -= model.BetAmount;
                await _context.SaveChangesAsync();

                // Zapisz sesję gry
                var gameSession = new GameSession
                {
                    UserId = user.id,
                    GameType = "Crash",
                    StartTime = DateTime.Now,
                    IsActive = true,
                    BetAmount = (decimal)model.BetAmount
                };
                
                _context.GameSessions.Add(gameSession);
                await _context.SaveChangesAsync();

                return Ok(new { 
                    success = true, 
                    newBalance = user.balance, 
                    sessionId = gameSession.Id 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Wystąpił błąd: {ex.Message}" });
            }
        }

        [HttpPost("collectWin")]
        public async Task<IActionResult> CollectWin([FromBody] WinModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Użytkownik niezalogowany" });
            }

            var user = await _context.DaneUzytkownikow.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { success = false, message = "Nie znaleziono użytkownika" });
            }

            var gameSession = await _context.GameSessions.FindAsync(model.SessionId);
            if (gameSession == null || gameSession.UserId != user.id || !gameSession.IsActive)
            {
                return BadRequest(new { success = false, message = "Nieprawidłowa sesja gry" });
            }

            try
            {
                // Oblicz wygraną na podstawie mnożnika
                decimal winAmount = (decimal)model.BetAmount * (decimal)model.Multiplier;
                
                // Dodaj wygraną do konta
                user.balance += (float)winAmount;
                
                // Zaktualizuj sesję gry
                gameSession.IsActive = false;
                gameSession.EndTime = DateTime.Now;
                gameSession.WinAmount = winAmount;
                
                await _context.SaveChangesAsync();

                return Ok(new { success = true, newBalance = user.balance });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Wystąpił błąd: {ex.Message}" });
            }
        }

        [HttpPost("gameOver")]
        public async Task<IActionResult> GameOver([FromBody] GameOverModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized(new { success = false, message = "Użytkownik niezalogowany" });
            }

            var gameSession = await _context.GameSessions.FindAsync(model.SessionId);
            if (gameSession == null || gameSession.UserId != userId || !gameSession.IsActive)
            {
                return BadRequest(new { success = false, message = "Nieprawidłowa sesja gry" });
            }

            try
            {
                // Zakończ sesję gry (przegrana)
                gameSession.IsActive = false;
                gameSession.EndTime = DateTime.Now;
                gameSession.WinAmount = 0;
                
                await _context.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Wystąpił błąd: {ex.Message}" });
            }
        }
    }

    public class BetModel
    {
        public float BetAmount { get; set; }
    }

    public class WinModel
    {
        public int SessionId { get; set; }
        public float BetAmount { get; set; }
        public float Multiplier { get; set; }
    }

    public class GameOverModel
    {
        public int SessionId { get; set; }
    }
}