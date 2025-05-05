using System;
using System.Collections.Generic;

namespace BlackBank.Models.ViewModels
{
    public class HistoriaTransakcjiViewModel
    {
        public List<TransakcjaViewModel> Transakcje { get; set; } = new List<TransakcjaViewModel>();
        public float AktualnySaldoKonta { get; set; }
        public string Waluta { get; set; } = "PLN";
    }
}