-- Dodawanie Administratorów
INSERT INTO Administratorzy (ImieNazwisko, Email, Haslo) VALUES
('Jan Kowalski', 'admin@example.com', 'admin123');

-- Dodawanie Uczestników
INSERT INTO Uczestnicy (ImieNazwisko, Email, Haslo) VALUES
('Anna Nowak', 'anna.nowak@example.com', 'haslo123'),
('Piotr Wiśniewski', 'piotr.wisniewski@example.com', 'haslo456');

-- Dodawanie Lokalizacji
INSERT INTO Lokalizacje (Nazwa, Adres, PojemnoscMaksymalna) VALUES
('Hala Sportowa', 'ul. Sportowa 1, Kraków', 500),
('Teatr Miejski', 'ul. Teatralna 5, Warszawa', 300);

-- Dodawanie Organizatorów
INSERT INTO Organizatorzy (NazwaFirmy, Kontakt) VALUES
('EventMasters', 'kontakt@eventmasters.pl'),
('CityEvents', 'biuro@cityevents.pl');

-- Dodawanie Wydarzeń
INSERT INTO Wydarzenia (Nazwa, Data, Godzina, Lokalizacja_ID, LiczbaMiejsc, Organizator_ID) VALUES
('Koncert Rockowy', '2024-06-15', '19:00', 1, 500, 1),
('Sztuka Teatralna', '2024-07-20', '18:30', 2, 300, 2);

-- Dodawanie Kategorii
INSERT INTO Kategorie (NazwaKategorii) VALUES
('Muzyka'),
('Teatr'),
('Sport'),
('Edukacja');

-- Powiązanie Wydarzeń z Kategoriami
INSERT INTO Wydarzenie_Kategoria (Wydarzenie_ID, Kategoria_ID) VALUES
(1, 1),  -- Koncert Rockowy -> Muzyka
(2, 2);  -- Sztuka Teatralna -> Teatr

