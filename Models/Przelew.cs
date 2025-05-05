using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BlackBank.Models
{
    public class Przelew
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NadawcaID { get; set; }

        [Required]
        public int OdbiorcaID { get; set; }

        [Required]
        public float Kwota { get; set; }

        [Required]
        public DateTime DataPrzelewu { get; set; } = DateTime.Now;
        public string Tytul = "Przelew";

        [ForeignKey("NadawcaID")]
        public virtual DaneUzytkownika Nadawca { get; set; }
        
        [ForeignKey("OdbiorcaID")]
        public virtual DaneUzytkownika Odbiorca { get; set; }
    }
}