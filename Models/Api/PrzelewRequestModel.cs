using System.ComponentModel.DataAnnotations;

namespace BlackBank.Models.Api
{
    public class PrzelewRequestModel
    {
        [Required(ErrorMessage = "Adres email odbiorcy jest wymagany.")]
        [EmailAddress(ErrorMessage = "Wprowadź poprawny adres email.")]
        [Display(Name = "Email odbiorcy")]
        public string EmailOdbiorcy { get; set; }

        [Required(ErrorMessage = "Kwota przelewu jest wymagana.")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Kwota musi być większa od 0.")]
        [Display(Name = "Kwota")]
        public float Kwota { get; set; }

        [Display(Name = "Tytuł przelewu")]
        public string Tytul { get; set; } = "Przelew";
    }
}