-- Tabela Uczestnicy
CREATE TABLE Uczestnicy (
    ID SERIAL PRIMARY KEY,
    ImieNazwisko TEXT NOT NULL,
    Email TEXT UNIQUE NOT NULL,
    Haslo TEXT NOT NULL
);

-- Tabela Lokalizacje
CREATE TABLE Lokalizacje (
    ID SERIAL PRIMARY KEY,
    Nazwa TEXT NOT NULL,
    Adres TEXT NOT NULL,
    PojemnoscMaksymalna INTEGER NOT NULL
);

-- Tabela Organizatorzy
CREATE TABLE Organizatorzy (
    ID SERIAL PRIMARY KEY,
    NazwaFirmy TEXT NOT NULL,
    Kontakt TEXT NOT NULL
);

-- Tabela Wydarzenia
CREATE TABLE Wydarzenia (
    ID SERIAL PRIMARY KEY,
    Nazwa TEXT NOT NULL,
    Data DATE NOT NULL,
    Godzina TIME NOT NULL,
    Lokalizacja_ID INTEGER NOT NULL REFERENCES Lokalizacje(ID),
    LiczbaMiejsc INTEGER NOT NULL,
    Organizator_ID INTEGER NOT NULL REFERENCES Organizatorzy(ID)
);


-- Tabela Rezerwacje
CREATE TABLE Rezerwacje (
    ID SERIAL PRIMARY KEY,
    Uczestnik_ID INTEGER NOT NULL REFERENCES Uczestnicy(ID),
    Wydarzenie_ID INTEGER NOT NULL REFERENCES Wydarzenia(ID),
    DataRezerwacji DATE NOT NULL
);

-- Tabela Kategorie
CREATE TABLE Kategorie (
    ID SERIAL PRIMARY KEY,
    NazwaKategorii TEXT NOT NULL UNIQUE
);

-- Tabela asocjacyjna Wydarzenie_Kategoria
CREATE TABLE Wydarzenie_Kategoria (
    ID SERIAL PRIMARY KEY,
    Wydarzenie_ID INTEGER NOT NULL REFERENCES Wydarzenia(ID),
    Kategoria_ID INTEGER NOT NULL REFERENCES Kategorie(ID),
    UNIQUE(Wydarzenie_ID, Kategoria_ID) -- Zapobiega duplikowaniu połączeń
);

--Tabela Administratorzy
CREATE TABLE Administratorzy (
    ID SERIAL PRIMARY KEY,
    ImieNazwisko VARCHAR(100) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Haslo VARCHAR(255) NOT NULL
);
