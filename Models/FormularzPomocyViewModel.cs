using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BlackBank.Models
{
    public class FormularzPomocyViewModel
    {
        [Required(ErrorMessage = "Opis problemu jest wymagany.")]
        public string OpisProblemu { get; set; }

        public IFormFile? Zalacznik { get; set; }
    }
}
