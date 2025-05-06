# BlackBank - Bank-Kasyno Online
## Autorzy
- Jędrzej Steckiewicz
- Jakub Siedlecki
- Marcin Sejbuk
## Opis projektu
BlackBank to interaktywna platforma kasyna online opracowana przy użyciu ASP.NET Core. Projekt oferuje różne gry hazardowe, z których główną jest „Crash" - dynamiczna gra, w której gracze obstawiają, jak wysoko wzrośnie mnożnik przed "crashem" (nagłym zatrzymaniem)

## Główne funkcje
System kont użytkowników
- Rejestracja i logowanie użytkowników
- Przechowywanie i zarządzanie saldem użytkowników
- Autoryzacja dostępu do gier
## Gra Crash
- Interaktywna wizualizacja wzrastającego mnożnika na dynamicznie rysowanym wykresie
- System stawiania zakładów i wypłacania wygranych
- Wizualne efekty wzrostu mnożnika i "crashu"
- Wyświetlanie poprzednich mnożników i liczników czasowych
- Integracja z bazą danych w celu zarządzania saldem użytkownika

## Funkcje społecznościowe
- Wyświetlanie liczby graczy online w każdej grze
- Statystyki aktywności użytkowników

## Technologie
- Back-end: ASP.NET Core MVC
- Front-end: HTML, CSS, JavaScript
- Baza danych: Entity Framework Core, Sqlite
- Animacje: Canvas HTML5

## Architektura projektu
### Kontrolery
- HomeController - zarządzanie stroną główną i wyświetlanie statystyk
- CasinoController - obsługa widoków gier kasynowych
- (Planowany) CasinoApiController - API do obsługi operacji finansowych w grach
### Modele
- DaneUzytkownika - dane użytkownika, w tym saldo
- GameSession - sesje gry, przechowujące informacje o zakładach i wygranych
### Widoki
- Responsywny interfejs użytkownika z dedykowanymi stylami CSS
- Dynamicznie aktualizowany interfejs gier za pomocą skryptów JavaScript
### System animacji
- Wykorzystanie Canvas HTML5 do renderowania animacji w grach
- Efekty wizualne takie jak pulsowanie i eksplozje
- Planowane rozszerzenia
- Pełna integracja balansu użytkownika z bazą danych w grze Crash
- Dodanie historii transakcji i statystyk dla użytkowników
- Rozbudowa istniejących i dodanie nowych gier kasynowych
- System czatu dla graczy
- Tabela rankingowa najlepszych graczy

## Zrzuty Ekranu
![image](https://github.com/user-attachments/assets/17392248-a5f6-4706-9023-1d9ac339998c)
![image](https://github.com/user-attachments/assets/05d40e5f-da3c-4342-b728-226da36627da)
![image](https://github.com/user-attachments/assets/d8db1657-c178-4521-ae10-6d96b064b10e)
