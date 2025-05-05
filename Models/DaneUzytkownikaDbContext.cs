using Microsoft.EntityFrameworkCore;

namespace BlackBank.Models
{
    public class DaneUzytkownikaDbContext : DbContext
    {



        public DaneUzytkownikaDbContext(DbContextOptions<DaneUzytkownikaDbContext> options)
            : base(options)
        {
                
        }

        public DbSet<DaneUzytkownika> DaneUzytkownikow { get; set; }
        public DbSet<Przelew> Przelewy { get; set; }

        public DbSet<GameSession> GameSessions { get; set; }
    }
}
