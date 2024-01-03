
# Serwisu internetowy do prezentacji wyników w grze Dutch

Serwis internetowy służący do przechowywania oraz prezentowania wyników gier rozgrywanych w ramach Tournament Dutch Federation. 

Serwis został napisany w ramach pracy inżynierskiej.


## Zawartość

Serwis posiada dużo widoków oraz proste menu na górze strony. Serwis pozwala użytkownikom na:

- Przeglądanie listy graczy
- Przeglądanie turniejów i lig, które  się odbyły, wraz z listą rozegranych w ich ramie rozgrywek.
- Uzyskanie informacji o federacji oraz zasadach gry Dutch
- Grę w Dutch z komputerowym przeciwnikiem


## Technologia

Serwer jest oparty na technologii .Net Framework 4.7.2. Wykorzystany został wzorzec projektowy MCV (Model-view-Controller) oraz komunikacja z bazą danych SQL za pomocą framework'u EntityFramework.


## Funkcjonalności administratora

Serwis pozwala na logowanie dla administartorów. Po zalogowaniu admini mają możliwość dodawania oraz edycji graczy, turniejów, lig i meczy.

## Plany rozwoju

Lista elementów do implementacji w przyszłości

- Lepsze przekierowania przy logowaniu
- Efekty specjalne kart
- Wygrana przy clean board

## Zmiana środowiska

Ze względów kosztów hosting'u rozważam zmianę używanej technologii na .Net Core.

## Błędy

Aktualnie znane błędy

- Po logowaniu nie wracasz na stronę przekierowania
- Dodanie meczu nie zmienia elo graczy
- Na ekranie mniejszym niż 992px pusta karta na początku wprowadza zamęt : Gra z botem
- Koniec kart w deck'u wyrzuca błąd : Gra z botem
- Więcej niż 8 kart wyrzuca błąd : Gra z botem
- Ogłoszenia powinny również zapisywać godzinę
- Zła responsywność okienek turniejów i ogłoszeń

## Licencja

GNU GPL-3.0 (General Public License)

## Autorzy

-  [Piotr Majewski](https://github.com/G4ND4LFpl)

