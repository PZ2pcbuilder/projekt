# 📘 Dokumentacja Projektu — PC Builder

## 1. Informacje ogólne

| Kategoria | Szczegóły |
|---|---|
| **Tytuł projektu** | **PC Builder (Konfigurator PC)** |
| **Autorzy** | Jakub Białek, Piotr Bera |
| **Technologia** | C#, ASP.NET Core MVC, Entity Framework Core, REST API |
| **Baza danych** | Relacyjna baza danych inicjalizowana z plików CSV |

---

# 2. Opis aplikacji

**PC Builder** to aplikacja webowa służąca do wirtualnego konfigurowania zestawów komputerowych.  
Głównym celem projektu jest ułatwienie użytkownikom doboru kompatybilnych podzespołów spośród szerokiej gamy komponentów komputerowych, takich jak:

- procesory (CPU),
- karty graficzne (GPU),
- płyty główne,
- pamięci RAM,
- obudowy,
- chłodzenia CPU,
- dyski,
- zasilacze.

Aplikacja wyróżnia się systemem sprawdzania kompatybilności sprzętowej w czasie rzeczywistym. Podczas dodawania kolejnych elementów do konfiguracji system automatycznie analizuje zgodność podzespołów i ukrywa lub odrzuca części, które nie pasują do już wybranych komponentów.

Dzięki systemowi kont użytkownika niezakończone konfiguracje są automatycznie zapisywane w bazie danych, co umożliwia kontynuowanie pracy po ponownym zalogowaniu.

---

# 3. Funkcjonalności

## 🔧 System kompatybilności sprzętowej

Silnik aplikacji analizuje wybory użytkownika na bieżąco, wykorzystując kluczowe parametry techniczne zapisane w modelach danych.

### Procesor ↔ Płyta główna
- Weryfikacja zgodności gniazda procesora (**Socket**).

### RAM ↔ Procesor i Płyta główna
- Sprawdzanie standardu pamięci (**DDR4 / DDR5**),
- kontrola maksymalnej obsługiwanej pojemności RAM,
- kontrola liczby dostępnych slotów pamięci.

### Obudowa ↔ Płyta główna
- Dopasowanie formatu płyty głównej:
  - ATX,
  - Micro ATX,
  - Mini ITX.

### Obudowa ↔ Karta graficzna (GPU)
- Weryfikacja długości karty graficznej względem maksymalnej przestrzeni dostępnej w obudowie.

### Obudowa ↔ Chłodzenie CPU
- Sprawdzanie wysokości chłodzenia procesora względem limitu obudowy.

### Obudowa ↔ Zasilacz
- Dopasowanie standardu zasilacza (**PSU Form Factor**).

### Kalkulator mocy (TDP)
System automatycznie:
- sumuje pobór energii komponentów,
- dodaje bezpieczny bufor mocy (**70W**),
- rekomenduje odpowiedni zasilacz dla konfiguracji.

---

## Zarządzanie konfiguracją i sesją

- Wybrane komponenty zapisywane są w sesji przeglądarki.
- Konfiguracja serializowana jest do formatu **JSON**.
- Po wylogowaniu konfiguracja zostaje trwale zapisana w bazie danych (`SavedConfigJson`).
- Użytkownik może wrócić do swojej konfiguracji po ponownym zalogowaniu.

---

## System kont, ról i autoryzacja

### Funkcje bezpieczeństwa
- Globalny filtr autoryzacji (`AuthFilter`) wymuszający logowanie.
- Bezpieczne haszowanie haseł przy użyciu algorytmu **BCrypt**.
- Logowanie obsługiwane przez REST API (`/api/auth/*`).

### Role użytkowników

| Rola | Uprawnienia |
|---|---|
| **User** | Standardowy użytkownik aplikacji |
| **Admin** | Zarządzanie użytkownikami i systemem oraz posiada te same funkcjonalności co *User*|

### Panel administratora

Administrator posiada możliwość:
- zarządzania użytkownikami,
- zmiany ról,
- resetowania haseł,
- usuwania kont.

---

## 🔎 Filtrowanie i wyszukiwanie

Każda kategoria komponentów posiada dedykowany model filtrów, np.:
- `CpuFilter`,
- `GpuFilter`.

### Dostępne opcje filtrowania

- wyszukiwanie tekstowe,
- filtrowanie cen,
- filtrowanie pojemności,
- filtrowanie wymiarów fizycznych,
- filtrowanie taktowania,
- filtrowanie poziomu hałasu,
- filtrowanie poboru mocy.

---

# 4. Sposób użycia — instrukcja krok po kroku

## 1️⃣ Rejestracja i logowanie

Uruchom aplikację i:
- utwórz nowe konto,
- lub zaloguj się na istniejące konto.

### Domyślne konto administratora

```text
Login: admin
Hasło: admin
```

---

## 2️⃣ Rozpoczęcie konfiguracji

Przejdź do zakładki **Mój zestaw**, gdzie znajduje się główny panel konfiguratora.

---

## 3️⃣ Wybór komponentów

Kliknij przycisk **„Dodaj”** przy interesującej kategorii podzespołów.

Przykładowe kategorie:
- CPU,
- GPU,
- RAM,
- Motherboard,
- PSU,
- Case.

---

## 4️⃣ Filtrowanie wyników

Skorzystaj z panelu filtrów, aby:
- ustawić budżet,
- wybrać producenta,
- określić socket,
- zawęzić parametry techniczne.

---

## 5️⃣ Dodawanie komponentów

Po wybraniu części:
- kliknij przycisk dodawania,
- wrócisz automatycznie do konfiguratora.

---

## 6️⃣ Inteligentna kompatybilność

Podczas budowy zestawu aplikacja:
- wyświetla komunikaty o zgodności,
- ukrywa niekompatybilne komponenty,
- automatycznie kontroluje ograniczenia sprzętowe.

---

## 7️⃣ Dobór zasilacza

Po dodaniu procesora i karty graficznej system:
- oblicza wymagane zapotrzebowanie energetyczne,
- rekomenduje odpowiednią moc zasilacza.

---

## 8️⃣ Edycja konfiguracji

Użytkownik może:
- usuwać pojedyncze komponenty,
- wyczyścić cały zestaw,
- zmienić dowolny element konfiguracji.

---

## 9️⃣ Wylogowanie i zapis konfiguracji

Po wylogowaniu:
- konfiguracja zostaje automatycznie zapisana,
- użytkownik może kontynuować pracę po ponownym zalogowaniu.

---

# 5. Podsumowanie

Projekt **PC Builder** stanowi kompleksowe narzędzie wspierające użytkownika w budowie kompatybilnego zestawu komputerowego. Dzięki wykorzystaniu technologii ASP.NET Core MVC, Entity Framework Core oraz inteligentnego systemu kompatybilności aplikacja zapewnia wygodne, bezpieczne i nowoczesne środowisko do konfiguracji komputerów.
