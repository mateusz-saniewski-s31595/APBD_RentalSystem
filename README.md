Wypożyczalnia Sprzętu

# Decyzje projektowe

Kod podzielony jest na trzy warstwy: Domain, Services, UI.

Klasy domenowe nie wiedzą nic o regułach biznesowych ani o wyświetlaniu.
Services zawiera logikę biznesową, ale nie wyświetla nic na ekranie.
ConsoleUI tylko formatuje i wyświetla dane otrzymane z serwisu.

Każda klasa ma jedną wyraźną odpowiedzialność. Rental przechowuje dane
wypożyczenia, ale nie oblicza kar — otrzymuje gotową kwotę z zewnątrz
i tylko ją zapisuje. RentalService koordynuje operacje i egzekwuje reguły,
ale nie wie jak wyglądają dane na ekranie. DailyRatePenaltyCalculator
robi dokładnie jedną rzecz: oblicza karę na podstawie daty zwrotu
i terminu.

Limity wypożyczeń są zdefiniowane bezpośrednio w klasach Student i Employee,
a nie w serwisie jako instrukcja if/else. Serwis pyta obiekt użytkownika o jego limit 
i nie musi znać szczegółów. Stawka kary to jeden parametr konstruktora DailyRatePenaltyCalculator. 
Zmiana którejkolwiek z tych reguł wymaga edycji dokładnie jednego miejsca w kodzie.

RentalService zależy od interfejsu IPenaltyCalculator, a nie od konkretnej
klasy. Dzięki temu podmiana reguły naliczania kar nie wymaga żadnych zmian
w serwisie — wystarczy dostarczyć inną implementację interfejsu. Klasy
domenowe nie mają żadnych zewnętrznych zależności i można je zrozumieć
oraz testować w izolacji.

Przewidywalne niepowodzenia biznesowe, takie jak przekroczenie limitu
wypożyczeń czy próba wypożyczenia zajętego sprzętu, są normalną
częścią aplikacji, a nie sytuacją wyjątkową. Zwrócenie obiektu
OperationResult jawnie sygnalizuje, że operacja może się nie powieść,
i wymusza obsługę tego przypadku przez wywołującego, bez ukrytego
przepływu sterowania przez wyjątki.


# Struktura projektu

RentalSystem/
├── Domain/
│   ├── Equipment.cs   - Hierarchia klas sprzętu
│   ├── User.cs        - Hierarchia klas użytkownika
│   └── Rental.cs      - Model wypożyczenia
├── Services/
│   ├── IPenaltyCalculator.cs          - Interfejs kalkulatora kar
│   ├── DailyRatePenaltyCalculator.cs  - Implementacja kary
│   ├── OperationResult.cs             - Typ wyniku operacji
│   └── RentalService.cs               - Logika biznesowa
├── UI/
│   └── ConsoleUI.cs   - Warstwa prezentacji
└── Program.cs
