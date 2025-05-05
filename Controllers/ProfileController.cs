using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlackBank.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using BlackBank.Models.Api;
using BlackBank.Models.ViewModels;

namespace BlackBank.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ILogger<ProfileController> _logger;
        private readonly DaneUzytkownikaDbContext _context;

        public ProfileController(ILogger<ProfileController> logger, DaneUzytkownikaDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Pomoc() // widok pomocy
        {
            return View("pomoc");
        }

        [HttpPost] // Po wyslaniu formularza
        public IActionResult Pomoc(string OpisProblemu)
        {
            TempData["NumerZgloszenia"] = DateTime.Now.ToString("yyyyMMddHHmmss");
            return RedirectToAction("Pomoc");
        }

        [HttpGet]
        public IActionResult Profil()
        {
            try
            {
                _logger.LogInformation("Wywołano akcję Profil w ProfileController");
                
                // Sprawdź, czy użytkownik jest uwierzytelniony
                _logger.LogInformation("Czy użytkownik jest uwierzytelniony: {isAuthenticated}", User.Identity.IsAuthenticated);
                
                var userId = HttpContext.Session.GetInt32("UserId");
                _logger.LogInformation("UserId z sesji: {userId}", userId);
                
                if (userId == null)
                {
                    _logger.LogWarning("Brak UserId w sesji - przekierowanie do strony głównej");
                    return RedirectToAction("Index", "Home");
                }
                
                var user = _context.DaneUzytkownikow.Find(userId);
                if (user == null)
                {
                    _logger.LogWarning("Nie znaleziono użytkownika o ID: {userId}", userId);
                    return NotFound();
                }
                
                _logger.LogInformation("Zwracam widok Profil dla użytkownika: {email}", user.email);
                return View(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd w akcji Profil");
                throw;
            }
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Wyświetlono profil użytkownika: {id}", HttpContext.Session.GetInt32("UserId"));
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = _context.DaneUzytkownikow.Find(userId);
            if (user == null)
            {
                return NotFound();
            }
            
            // Jawnie wskaż, który widok ma być użyty
            return View("Profil", user);
        }

        public IActionResult Przelew()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> WykonajPrzelew(PrzelewRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Przelew", model);
            }

            // Pobierz id zalogowanego użytkownika
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                ModelState.AddModelError("", "Nie można zidentyfikować użytkownika.");
                return View("Przelew", model);
            }

            // Znajdź nadawcę
            var nadawca = await _context.DaneUzytkownikow.FindAsync(userId);
            if (nadawca == null)
            {
                ModelState.AddModelError("", "Nie znaleziono danych użytkownika.");
                return View("Przelew", model);
            }

            // Znajdź odbiorcę po emailu
            var odbiorca = await _context.DaneUzytkownikow
                .FirstOrDefaultAsync(u => u.email == model.EmailOdbiorcy);
            
            if (odbiorca == null)
            {
                ModelState.AddModelError("EmailOdbiorcy", "Nie znaleziono użytkownika o podanym adresie email.");
                return View("Przelew", model);
            }

            // Sprawdź czy użytkownik nie próbuje wysłać przelewu do siebie
            if (odbiorca.id == userId)
            {
                ModelState.AddModelError("EmailOdbiorcy", "Nie możesz wykonać przelewu do własnego konta.");
                return View("Przelew", model);
            }

            // Sprawdź czy nadawca ma wystarczające środki
            if (nadawca.balance < model.Kwota)
            {
                ModelState.AddModelError("Kwota", "Nie masz wystarczających środków na koncie.");
                return View("Przelew", model);
            }

            try
            {
                // Wykonaj przelew bezpośrednio, bez wywoływania API
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Aktualizuj stan konta nadawcy i odbiorcy
                        nadawca.balance -= model.Kwota;
                        odbiorca.balance += model.Kwota;

                        _context.DaneUzytkownikow.Update(nadawca);
                        _context.DaneUzytkownikow.Update(odbiorca);

                        // Utwórz rekord przelewu
                        var przelew = new Przelew
                        {
                            NadawcaID = nadawca.id,
                            OdbiorcaID = odbiorca.id,
                            Kwota = model.Kwota,
                            Tytul = model.Tytul ?? "Przelew",
                            DataPrzelewu = DateTime.Now
                        };

                        _context.Przelewy.Add(przelew);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        TempData["SuccessMessage"] = "Przelew wykonany pomyślnie!";
                        return RedirectToAction("Index");
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Wystąpił błąd podczas wykonywania przelewu: {ex.Message}");
                return View("Przelew", model);
            }
        }

        public async Task<IActionResult> HistoriaPrzelewow()
        {
            // Pobierz id zalogowanego użytkownika
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Nie można zidentyfikować użytkownika.";
                return RedirectToAction("Index");
            }

            try
            {
                // Znajdź dane użytkownika
                var user = await _context.DaneUzytkownikow.FindAsync(userId);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Nie znaleziono danych użytkownika.";
                    return RedirectToAction("Index");
                }

                // Pobierz przelewy wychodzące
                var przelewyWychodzace = await _context.Przelewy
                    .Where(p => p.NadawcaID == userId)
                    .Include(p => p.Odbiorca)
                    .OrderByDescending(p => p.DataPrzelewu)
                    .Select(p => new TransakcjaViewModel
                    {
                        Id = p.Id,
                        Typ = "Wychodzący",
                        Kontrahent = $"{p.Odbiorca.name} {p.Odbiorca.surname}",
                        Kwota = p.Kwota,
                        DataPrzelewu = p.DataPrzelewu,
                        Tytul = p.Tytul
                    })
                    .ToListAsync();

                // Pobierz przelewy przychodzące
                var przelewyPrzychodzace = await _context.Przelewy
                    .Where(p => p.OdbiorcaID == userId)
                    .Include(p => p.Nadawca)
                    .OrderByDescending(p => p.DataPrzelewu)
                    .Select(p => new TransakcjaViewModel
                    {
                        Id = p.Id,
                        Typ = "Przychodzący",
                        Kontrahent = $"{p.Nadawca.name} {p.Nadawca.surname}",
                        Kwota = p.Kwota,
                        DataPrzelewu = p.DataPrzelewu,
                        Tytul = p.Tytul
                    })
                    .ToListAsync();

                // Połącz listy i posortuj według daty (najnowsze najpierw)
                var wszystkiePrzelewy = przelewyWychodzace
                    .Concat(przelewyPrzychodzace)
                    .OrderByDescending(p => p.DataPrzelewu)
                    .ToList();

                // Utwórz model widoku
                var viewModel = new HistoriaTransakcjiViewModel
                {
                    Transakcje = wszystkiePrzelewy,
                    Waluta = "PLN",
                    AktualnySaldoKonta = user.balance
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas pobierania historii przelewów");
                TempData["ErrorMessage"] = $"Wystąpił błąd podczas pobierania historii przelewów: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}