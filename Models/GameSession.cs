using System;
using System.ComponentModel.DataAnnotations;

namespace BlackBank.Models
{
    public class GameSession
    {
        [Key]
        public int Id { get; set; }
        
        public int UserId { get; set; }
        
        [Required]
        public string GameType { get; set; }
        
        public DateTime StartTime { get; set; }
        
        public DateTime? EndTime { get; set; }
        
        public bool IsActive { get; set; }
        
        public decimal BetAmount { get; set; }
        
        public decimal? WinAmount { get; set; }
        
        // Navigation property
        public virtual DaneUzytkownika User { get; set; }
    }
}