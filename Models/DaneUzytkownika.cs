using System.ComponentModel.DataAnnotations;

namespace BlackBank.Models
{
    public class DaneUzytkownika
    {
        [Key]
        public int id { get; set; }

        [Required]
        public string email { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string surname { get; set; }

        [Required]
        public string password { get; set; }

        [Required]
        public string gender { get; set; }

        [Required]
        [Range(18, 99, ErrorMessage = "Wiek musi być pomiędzy 18 a 99 lat.")]
        public int age { get; set; }

        public float balance { get; set; } = 1000.0f; 

        public bool isActive { get; set; } = true;

        
    }
}
