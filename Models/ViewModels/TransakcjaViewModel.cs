using System;
using System.Collections.Generic;

namespace BlackBank.Models.ViewModels
{
    public class TransakcjaViewModel
    {
        public int Id{get;set;}
        public string Typ{get;set;}
        public string Kontrahent{get;set;}
        public float Kwota{get;set;}
        public DateTime DataPrzelewu{get;set;}
        public string Tytul{get;set;}
        
    }
}