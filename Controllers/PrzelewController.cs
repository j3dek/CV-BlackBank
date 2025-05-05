using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlackBank.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using BlackBank.Models.Api;

namespace BlackBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrzelewController : ControllerBase
    {
        private readonly DaneUzytkownikaDbContext _context;
        public PrzelewController(DaneUzytkownikaDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> WykonajPrzelew(PrzelewRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Bezpieczniejsze parsowanie ID użytkownika
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Nie można zidentyfikować użytkownika.");
            }
            
            var nadawca = await _context.DaneUzytkownikow.FindAsync(userId);
            if (nadawca == null)
            {
                return NotFound("Nadawca nie znaleziony.");
            }

            // Znajdź odbiorcę po emailu zamiast po ID
            var odbiorca = await _context.DaneUzytkownikow
                .FirstOrDefaultAsync(u => u.email == model.EmailOdbiorcy);
            if (odbiorca == null)
            {
                return NotFound("Odbiorca nie znaleziony.");
            }

            // Sprawdź czy nadawca i odbiorca to różne osoby
            if (odbiorca.id == userId)
            {
                return BadRequest("Nie możesz wykonać przelewu do własnego konta.");
            }

            // Sprawdzenie czy nadawca ma wystarczające środki
            if (nadawca.balance < model.Kwota)
            {
                return BadRequest("Niewystarczające środki na koncie.");
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    nadawca.balance -= model.Kwota;
                    odbiorca.balance += model.Kwota;

                    _context.DaneUzytkownikow.Update(nadawca);
                    _context.DaneUzytkownikow.Update(odbiorca);

                    var przelew = new Przelew
                    {
                        NadawcaID = userId,
                        OdbiorcaID = odbiorca.id, // Użyj ID znalezionego odbiorcy
                        Kwota = model.Kwota,
                        Tytul = model.Tytul,
                        DataPrzelewu = DateTime.Now
                    };
                    _context.Przelewy.Add(przelew);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return Ok(new { 
                        message = "Przelew wykonany pomyślnie.",
                        nowyStanKonta = nadawca.balance 
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return StatusCode(500, $"Wystąpił błąd podczas wykonywania przelewu: {ex.Message}");
                }
            }
        }

        [HttpGet("historia")]
        public async Task<IActionResult> HistoriaPrzelewow()
        {
            // Bezpieczniejsze parsowanie ID użytkownika
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Nie można zidentyfikować użytkownika.");
            }

            var przelewyWychodzace = await _context.Przelewy
                .Where(p => p.NadawcaID == userId)  // Poprawione - użycie int zamiast string
                .Include(p => p.Odbiorca)
                .OrderByDescending(p => p.DataPrzelewu)
                .Select(p => new
                {
                    p.Id,
                    Odbiorca = $"{p.Odbiorca.name} {p.Odbiorca.surname}",
                    p.Kwota,
                    p.DataPrzelewu,
                    p.Tytul
                })
                .ToListAsync();

            var przelewyPrzychodzace = await _context.Przelewy
                .Where(p => p.OdbiorcaID == userId)  // Poprawione - użycie int zamiast string
                .Include(p => p.Nadawca)
                .OrderByDescending(p => p.DataPrzelewu)
                .Select(p => new
                {
                    p.Id,
                    Nadawca = $"{p.Nadawca.name} {p.Nadawca.surname}",
                    p.Kwota,
                    p.DataPrzelewu,
                    p.Tytul
                })
                .ToListAsync();

            // Tworzenie wspólnej listy przelewów
            var wszystkiePrzelewWychodzace = przelewyWychodzace
                .Select(p => new
                {
                    p.Id,
                    Kontrahent = p.Odbiorca,
                    p.Kwota,
                    p.DataPrzelewu,
                    p.Tytul,
                    Typ = "Wychodzący"
                });

            var wszystkiePrzelewPrzychodzace = przelewyPrzychodzace
                .Select(p => new
                {
                    p.Id,
                    Kontrahent = p.Nadawca,
                    p.Kwota,
                    p.DataPrzelewu,
                    p.Tytul,
                    Typ = "Przychodzący"
                });

            var wszystkiePrzelewy = wszystkiePrzelewWychodzace
                .Concat(wszystkiePrzelewPrzychodzace)
                .OrderByDescending(p => p.DataPrzelewu)
                .ToList();

            return Ok(new { przelewy = wszystkiePrzelewy });
        }
    }
}
