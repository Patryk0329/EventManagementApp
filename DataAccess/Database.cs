using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using EventManagementApp.Models;
using Npgsql;

namespace EventManagementApp.DataAccess
{
    public class Database
    {
        /// <summary>
        /// Łańcuch połączenia z bazą danych PostgreSQL.
        /// </summary>
        private readonly string connectionString;

        public Database()
        {
            connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string is not set in environment variables.");
            }
        }


        /// <summary>
        /// Pobiera listę wszystkich wydarzeń z bazy danych, łącznie z informacjami o lokalizacji, organizatorze i kategoriach.
        /// </summary>
        /// <returns>Lista obiektów <see cref="Wydarzenie"/> zawierająca szczegóły wydarzeń.</returns>
        public List<Wydarzenie> GetWydarzenia()
        {
            var wydarzenia = new List<Wydarzenie>();

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var query = @"
                SELECT 
                    w.ID, 
                    w.Nazwa, 
                    w.Data, 
                    w.Godzina, 
                    l.Nazwa AS LokalizacjaNazwa, 
                    l.Adres AS LokalizacjaAdres, 
                    w.LiczbaMiejsc, 
                    o.NazwaFirmy AS OrganizatorNazwa,
                    o.Kontakt AS OrganizatorKontakt,
                    STRING_AGG(k.NazwaKategorii, ', ') AS KategoriaNazwa
                FROM 
                    Wydarzenia w
                INNER JOIN 
                    Lokalizacje l ON w.Lokalizacja_ID = l.ID
                INNER JOIN 
                    Organizatorzy o ON w.Organizator_ID = o.ID
                LEFT JOIN 
                    Wydarzenie_Kategoria wk ON w.ID = wk.Wydarzenie_ID
                LEFT JOIN 
                    Kategorie k ON wk.Kategoria_ID = k.ID
                GROUP BY 
                    w.ID, l.Nazwa, l.Adres, o.NazwaFirmy, o.Kontakt, w.Nazwa, w.Data, w.Godzina, w.LiczbaMiejsc";

                    using (var command = new NpgsqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var wydarzenie = new Wydarzenie
                            {
                                ID = reader.GetInt32(0),
                                Nazwa = reader.GetString(1),
                                Data = reader.GetDateTime(2),
                                Godzina = reader.GetTimeSpan(3),
                                LokalizacjaNazwa = $"{reader.GetString(4)} ({reader.GetString(5)})",
                                LiczbaMiejsc = reader.GetInt32(6),
                                OrganizatorNazwa = reader.GetString(7),
                                OrganizatorKontakt = reader.GetString(8),
                                KategoriaNazwa = reader.IsDBNull(9) ? "Brak kategorii" : reader.GetString(9)
                            };
                            wydarzenia.Add(wydarzenie);
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas pobierania wydarzeń z bazy danych: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas pobierania wydarzeń: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return wydarzenia;
        }

        /// <summary>
        /// Pobiera listę wydarzeń przypisanych do określonej kategorii.
        /// </summary>
        /// <param name="kategoriaId">Identyfikator kategorii, według której filtrujemy wydarzenia.</param>
        /// <returns>Lista obiektów <see cref="Wydarzenie"/> zawierająca szczegóły wydarzeń dla danej kategorii.</returns>
        public List<Wydarzenie> GetWydarzeniaByKategoria(int kategoriaId)
        {
            var wydarzenia = new List<Wydarzenie>();

            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
            SELECT 
                w.ID, 
                w.Nazwa, 
                w.Data, 
                w.Godzina, 
                l.Nazwa AS LokalizacjaNazwa, 
                l.Adres AS LokalizacjaAdres, 
                w.LiczbaMiejsc, 
                o.NazwaFirmy AS OrganizatorNazwa,
                o.Kontakt AS OrganizatorKontakt,
                k.NazwaKategorii AS KategoriaNazwa
            FROM 
                Wydarzenia w
            INNER JOIN 
                Lokalizacje l ON w.Lokalizacja_ID = l.ID
            INNER JOIN 
                Organizatorzy o ON w.Organizator_ID = o.ID
            LEFT JOIN 
                Wydarzenie_Kategoria wk ON w.ID = wk.Wydarzenie_ID
            LEFT JOIN 
                Kategorie k ON wk.Kategoria_ID = k.ID
            WHERE 
                wk.Kategoria_ID = @KategoriaId";


                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KategoriaId", kategoriaId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            wydarzenia.Add(new Wydarzenie
                            {
                                ID = reader.GetInt32(0),
                                Nazwa = reader.GetString(1),
                                Data = reader.GetDateTime(2),
                                Godzina = reader.GetTimeSpan(3),
                                LokalizacjaNazwa = $"{reader.GetString(4)} ({reader.GetString(5)})",
                                LiczbaMiejsc = reader.GetInt32(6),
                                OrganizatorNazwa = reader.GetString(7),
                                OrganizatorKontakt = reader.GetString(8),
                                KategoriaNazwa = reader.IsDBNull(9) ? "Brak kategorii" : reader.GetString(9)
                            });
                        }
                    }
                }
            }

            return wydarzenia;
        }



        /// <summary>
        /// Rejestruje nowego uczestnika w bazie danych.
        /// </summary>
        /// <param name="imieNazwisko">Imię i nazwisko uczestnika.</param>
        /// <param name="email">Adres e-mail uczestnika.</param>
        /// <param name="haslo">Hasło uczestnika (zostanie zaszyfrowane przed zapisaniem).</param>
        /// <returns>Wartość logiczna wskazująca, czy rejestracja zakończyła się sukcesem.</returns>
        public bool ZarejestrujUczestnika(string imieNazwisko, string email, string haslo)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "INSERT INTO Uczestnicy (ImieNazwisko, Email, Haslo) VALUES (@ImieNazwisko, @Email, @Haslo)";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ImieNazwisko", imieNazwisko);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Haslo", HashPassword(haslo));
                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas rejestracji uczestnika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas rejestracji uczestnika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Generuje skrót hasła przy użyciu algorytmu SHA-256.
        /// </summary>
        /// <param name="haslo">Hasło w formie tekstowej do zaszyfrowania.</param>
        /// <returns>Skrót hasła w formacie Base64.</returns>
        private string HashPassword(string haslo)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(haslo);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

        /// <summary>
        /// Pobiera identyfikator uczestnika na podstawie jego adresu e-mail.
        /// </summary>
        /// <param name="email">Adres e-mail uczestnika.</param>
        /// <returns>Identyfikator uczestnika lub -1, jeśli uczestnika nie znaleziono.</returns>
        public int GetUczestnikId(string email)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT ID FROM Uczestnicy WHERE Email = @Email";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);
                        var result = command.ExecuteScalar();

                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            return -1;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas pobierania ID uczestnika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas pobierania ID uczestnika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return -1;
            }
        }

        /// <summary>
        /// Loguje uczestnika na podstawie adresu e-mail i hasła.
        /// </summary>
        /// <param name="email">Adres e-mail uczestnika.</param>
        /// <param name="haslo">Hasło uczestnika.</param>
        /// <returns>Wartość logiczna wskazująca, czy logowanie zakończyło się sukcesem.</returns>
        public bool ZalogujUczestnika(string email, string haslo)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT Haslo FROM Uczestnicy WHERE Email = @Email";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            string storedHashedPassword = result.ToString();
                            string hashedInputPassword = HashPassword(haslo);
                            return storedHashedPassword == hashedInputPassword;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas logowania: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Loguje administratora na podstawie adresu e-mail i hasła.
        /// </summary>
        /// <param name="email">Adres e-mail administratora.</param>
        /// <param name="haslo">Hasło administratora.</param>
        /// <returns>Wartość logiczna wskazująca, czy logowanie zakończyło się sukcesem.</returns>
        public bool ZalogujAdmina(string email, string haslo)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "SELECT Haslo FROM Administratorzy WHERE Email = @Email";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Email", email);

                        var result = command.ExecuteScalar();
                        if (result != null)
                        {
                            string storedPassword = result.ToString();
                            return storedPassword == haslo;
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas logowania uczestnika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas logowania uczestnika: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return false;
        }

        /// <summary>
        /// Rejestruje uczestnika na wybrane wydarzenie.
        /// </summary>
        /// <param name="uczestnikId">Identyfikator uczestnika.</param>
        /// <param name="wydarzenieId">Identyfikator wydarzenia.</param>
        /// <returns>Wartość logiczna wskazująca, czy rejestracja na wydarzenie zakończyła się sukcesem.</returns>
        public bool ZarezerwujMiejsce(int uczestnikId, int wydarzenieId)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // Sprawdzenie, czy uczestnik jest już zapisany na to wydarzenie
                    var checkReservationQuery = @"
                SELECT COUNT(*) 
                FROM Rezerwacje 
                WHERE Uczestnik_ID = @UczestnikId AND Wydarzenie_ID = @WydarzenieId";

                    using (var checkReservationCommand = new NpgsqlCommand(checkReservationQuery, connection))
                    {
                        checkReservationCommand.Parameters.AddWithValue("@UczestnikId", uczestnikId);
                        checkReservationCommand.Parameters.AddWithValue("@WydarzenieId", wydarzenieId);

                        int isAlreadyReserved = Convert.ToInt32(checkReservationCommand.ExecuteScalar());

                        if (isAlreadyReserved > 0)
                        {
                            MessageBox.Show("Już zapisałeś się na to wydarzenie.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }

                    // Sprawdzenie liczby dostępnych miejsc
                    var checkQuery = @"
                SELECT LiczbaMiejsc 
                FROM Wydarzenia 
                WHERE ID = @WydarzenieId";

                    using (var checkCommand = new NpgsqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@WydarzenieId", wydarzenieId);
                        int liczbaMiejsc = Convert.ToInt32(checkCommand.ExecuteScalar());

                        if (liczbaMiejsc <= 0)
                        {
                            MessageBox.Show("Brak dostępnych miejsc na to wydarzenie.", "Brak miejsc", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }

                    // Dodanie rezerwacji i aktualizacja liczby miejsc
                    var insertQuery = @"
                INSERT INTO Rezerwacje (Uczestnik_ID, Wydarzenie_ID, DataRezerwacji)
                VALUES (@UczestnikId, @WydarzenieId, @DataRezerwacji);

                UPDATE Wydarzenia
                SET LiczbaMiejsc = LiczbaMiejsc - 1
                WHERE ID = @WydarzenieId AND LiczbaMiejsc > 0";

                    using (var command = new NpgsqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@UczestnikId", uczestnikId);
                        command.Parameters.AddWithValue("@WydarzenieId", wydarzenieId);
                        command.Parameters.AddWithValue("@DataRezerwacji", DateTime.Now.Date);

                        command.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas zapisywania rezerwacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas zapisywania rezerwacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Pobiera listę wydarzeń, na które zapisał się dany uczestnik.
        /// </summary>
        /// <param name="uczestnikId">Identyfikator uczestnika.</param>
        /// <returns>Lista obiektów zawierających szczegóły wydarzeń zapisanych przez uczestnika.</returns>
        public List<object> GetWydarzeniaUczestnika(int uczestnikId)
        {
            var result = new List<object>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
            SELECT w.ID, w.Nazwa, w.Data, w.Godzina, l.Nazwa AS Lokalizacja
            FROM Rezerwacje r
            JOIN Wydarzenia w ON r.Wydarzenie_ID = w.ID
            JOIN Lokalizacje l ON w.Lokalizacja_ID = l.ID
            WHERE r.Uczestnik_ID = @UczestnikId";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UczestnikId", uczestnikId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new
                            {
                                ID = reader.GetInt32(0),
                                Nazwa = reader.GetString(1),
                                Data = reader.GetDateTime(2),
                                Godzina = reader.GetTimeSpan(3),
                                Lokalizacja = reader.GetString(4)
                            });
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Pobiera listę najpopularniejszych wydarzeń, sortowaną według liczby rezerwacji.
        /// </summary>
        /// <returns>Lista obiektów zawierających szczegóły najpopularniejszych wydarzeń.</returns>
        public List<object> GetNajpopularniejszeWydarzenia()
        {
            var result = new List<object>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
            SELECT w.ID, w.Nazwa, w.Data, w.Godzina, l.Nazwa, COUNT(r.Uczestnik_ID) AS LiczbaRezerwacji
            FROM Rezerwacje r
            JOIN Wydarzenia w ON r.Wydarzenie_ID = w.ID
            JOIN Lokalizacje l ON w.Lokalizacja_ID = l.ID
            GROUP BY w.ID, w.Nazwa, l.Nazwa
            ORDER BY LiczbaRezerwacji DESC";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new
                            {
                                ID = reader.GetInt32(0),
                                Nazwa = reader.GetString(1),
                                Data = reader.GetDateTime(2),
                                Godzina = reader.GetTimeSpan(3),
                                Lokalizacja = reader.GetString(4),
                                LiczbaRezerwacji = reader.GetInt32(5)
                            });
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Pobiera listę wydarzeń, które mają wolne miejsca.
        /// </summary>
        /// <returns>Lista obiektów zawierających szczegóły wydarzeń z wolnymi miejscami.</returns>
        public List<object> GetWolneMiejscaWydarzenia()
        {
            var result = new List<object>();
            using (var connection = new NpgsqlConnection(connectionString))
            {
                connection.Open();
                var query = @"
            SELECT w.ID, w.Nazwa, w.Data, w.Godzina, w.LiczbaMiejsc, l.Nazwa AS Lokalizacja
            FROM Wydarzenia w
            JOIN Lokalizacje l ON w.Lokalizacja_ID = l.ID
            WHERE w.LiczbaMiejsc > 0";

                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            result.Add(new
                            {
                                ID = reader.GetInt32(0),
                                Nazwa = reader.GetString(1),
                                Data = reader.GetDateTime(2),
                                Godzina = reader.GetTimeSpan(3),
                                LiczbaMiejsc = reader.GetInt32(4),
                                Lokalizacja = reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return result;
        }


        /// <summary>
        /// Dodaje nowego organizatora do bazy danych.
        /// </summary>
        /// <param name="nazwaFirmy">Nazwa firmy organizatora.</param>
        /// <param name="kontakt">Dane kontaktowe organizatora.</param>
        /// <returns>Wartość logiczna wskazująca, czy operacja zakończyła się sukcesem.</returns>
        public bool DodajOrganizatora(string nazwaFirmy, string kontakt)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Organizatorzy (NazwaFirmy, Kontakt) VALUES (@NazwaFirmy, @Kontakt)";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NazwaFirmy", nazwaFirmy);
                        cmd.Parameters.AddWithValue("@Kontakt", kontakt);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas dodawania organizatora: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas dodawania organizatora: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        /// <summary>
        /// Sprawdza, czy dane firmy są prawidłowe i istnieją w bazie danych.
        /// </summary>
        /// <param name="nazwaFirmy">Nazwa firmy organizatora.</param>
        /// <param name="kontakt">Dane kontaktowe organizatora.</param>
        /// <returns>Wartość logiczna wskazująca, czy firma istnieje w bazie danych.</returns>
        public bool SprawdzDaneFirmy(string nazwaFirmy, string kontakt)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    var query = "SELECT COUNT(*) FROM Organizatorzy WHERE NazwaFirmy = @Nazwa AND Kontakt = @Kontakt";

                    using (var command = new NpgsqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Nazwa", nazwaFirmy);
                        command.Parameters.AddWithValue("@Kontakt", kontakt);

                        long count = (long)command.ExecuteScalar();

                        Console.WriteLine($"Zapytanie: {query}");
                        Console.WriteLine($"Parametry: Nazwa={nazwaFirmy}, Kontakt={kontakt}");

                        return count > 0;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas sprawdzania danych firmy: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas sprawdzania danych firmy: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }



        /// <summary>
        /// Dodaje nowe wydarzenie do bazy danych na podstawie danych organizatora i szczegółów wydarzenia.
        /// </summary>
        /// <param name="nazwaFirmy">Nazwa firmy organizatora.</param>
        /// <param name="kontakt">Dane kontaktowe organizatora.</param>
        /// <param name="nazwaWydarzenia">Nazwa wydarzenia.</param>
        /// <param name="data">Data wydarzenia.</param>
        /// <param name="godzina">Godzina rozpoczęcia wydarzenia.</param>
        /// <param name="lokalizacja">Nazwa lokalizacji wydarzenia.</param>
        /// <param name="liczbaMiejsc">Liczba miejsc dostępnych na wydarzenie.</param>
        /// <param name="kategorie">Lista kategorii przypisanych do wydarzenia.</param>
        public void DodajWydarzenie(string nazwaFirmy, string kontakt, string nazwaWydarzenia, DateTime data, string godzina, string lokalizacja, int liczbaMiejsc, List<Kategoria> kategorie)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    try
                    {
                        // Pobranie ID organizatora
                        var getFirmIdQuery = "SELECT Id FROM Organizatorzy WHERE NazwaFirmy = @Nazwa AND Kontakt = @Kontakt";
                        int firmaId;
                        using (var getFirmIdCommand = new NpgsqlCommand(getFirmIdQuery, connection, transaction))
                        {
                            getFirmIdCommand.Parameters.AddWithValue("@Nazwa", nazwaFirmy);
                            getFirmIdCommand.Parameters.AddWithValue("@Kontakt", kontakt);

                            object firmaIdResult = getFirmIdCommand.ExecuteScalar() ?? throw new Exception("Nie znaleziono organizatora z podaną nazwą firmy i kontaktem.");

                            firmaId = Convert.ToInt32(firmaIdResult);
                        }

                        // Pobranie ID lokalizacji i maksymalnej pojemności
                        var getLocationIdQuery = "SELECT Id, PojemnoscMaksymalna FROM Lokalizacje WHERE Nazwa = @Lokalizacja";
                        int lokalizacjaId;
                        int pojemnoscMaksymalna;
                        using (var getLocationIdCommand = new NpgsqlCommand(getLocationIdQuery, connection, transaction))
                        {
                            getLocationIdCommand.Parameters.AddWithValue("@Lokalizacja", lokalizacja);
                            using (var reader = getLocationIdCommand.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new Exception("Nie znaleziono lokalizacji o podanej nazwie.");

                                lokalizacjaId = reader.GetInt32(0);
                                pojemnoscMaksymalna = reader.GetInt32(1);
                            }
                        }

                        // Sprawdzenie pojemności lokalizacji
                        if (liczbaMiejsc > pojemnoscMaksymalna)
                            throw new Exception("Przekroczono maksymalną pojemność lokalizacji.");

                        // Wstawienie wydarzenia
                        var insertEventQuery = "INSERT INTO Wydarzenia (Nazwa, Data, Godzina, Lokalizacja_Id, Organizator_Id, LiczbaMiejsc) " +
                                               "VALUES (@Nazwa, @Data, @Godzina, @LokalizacjaId, @FirmaId, @LiczbaMiejsc) RETURNING Id";

                        int wydarzenieId;
                        using (var insertEventCommand = new NpgsqlCommand(insertEventQuery, connection, transaction))
                        {
                            insertEventCommand.Parameters.AddWithValue("@Nazwa", nazwaWydarzenia);
                            insertEventCommand.Parameters.AddWithValue("@Data", data);
                            insertEventCommand.Parameters.AddWithValue("@Godzina", TimeSpan.Parse(godzina));
                            insertEventCommand.Parameters.AddWithValue("@LokalizacjaId", lokalizacjaId);
                            insertEventCommand.Parameters.AddWithValue("@FirmaId", firmaId);
                            insertEventCommand.Parameters.AddWithValue("@LiczbaMiejsc", liczbaMiejsc);

                            object eventIdResult = insertEventCommand.ExecuteScalar() ?? throw new Exception("Błąd podczas dodawania wydarzenia do bazy danych.");

                            wydarzenieId = Convert.ToInt32(eventIdResult);
                        }

                        // Wstawienie kategorii wydarzenia
                        foreach (var kategoria in kategorie)
                        {
                            var getCategoryIdQuery = "SELECT Id FROM Kategorie WHERE NazwaKategorii = @Kategoria";
                            int kategoriaId;
                            using (var getCategoryIdCommand = new NpgsqlCommand(getCategoryIdQuery, connection, transaction))
                            {
                                getCategoryIdCommand.Parameters.AddWithValue("@Kategoria", kategoria.Nazwa);

                                object categoryIdResult = getCategoryIdCommand.ExecuteScalar() ?? throw new Exception($"Nie znaleziono kategorii o nazwie: {kategoria.Nazwa}");

                                kategoriaId = Convert.ToInt32(categoryIdResult);
                            }

                            var insertCategoryEventQuery = "INSERT INTO Wydarzenie_Kategoria (Wydarzenie_ID, Kategoria_ID) " +
                                                           "VALUES (@WydarzenieId, @KategoriaId)";
                            using (var insertCategoryEventCommand = new NpgsqlCommand(insertCategoryEventQuery, connection, transaction))
                            {
                                insertCategoryEventCommand.Parameters.AddWithValue("@WydarzenieId", wydarzenieId);
                                insertCategoryEventCommand.Parameters.AddWithValue("@KategoriaId", kategoriaId);
                                insertCategoryEventCommand.ExecuteNonQuery();
                            }
                        }

                        // Zatwierdzenie transakcji
                        transaction.Commit();
                        MessageBox.Show("Wydarzenie zostało dodane pomyślnie!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas dodawania wydarzenia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas dodawania wydarzenia: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Pobiera listę wszystkich dostępnych kategorii z bazy danych.
        /// </summary>
        /// <returns>Lista obiektów reprezentujących kategorie.</returns>
        public List<Kategoria> GetKategorie()
            {
            List<Kategoria> kategorie = new List<Kategoria>();
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, NazwaKategorii FROM Kategorie";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            kategorie.Add(new Kategoria
                            {
                                ID = reader.GetInt32(0),
                                Nazwa = reader.GetString(1)
                            });
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas pobierania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas pobierania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return kategorie;
        }

        /// <summary>
        /// Usuwa kategorię z bazy danych na podstawie jej ID.
        /// </summary>
        /// <param name="id">ID kategorii do usunięcia.</param>
        public void UsunKategoria(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Kategorie WHERE ID = @id";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas usuwania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas usuwania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Dodaje nową kategorię do bazy danych.
        /// </summary>
        /// <param name="nazwa">Nazwa nowej kategorii.</param>
        public void DodajKategoria(string nazwa)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Kategorie (NazwaKategorii) VALUES (@nazwa)";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nazwa", nazwa);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas dodawania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas dodawania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Edytuje nazwę istniejącej kategorii w bazie danych.
        /// </summary>
        /// <param name="id">ID edytowanej kategorii.</param>
        /// <param name="nowaNazwa">Nowa nazwa kategorii.</param>
        public void EdytujKategoria(int id, string nowaNazwa)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Kategorie SET NazwaKategorii = @nazwa WHERE ID = @id";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nazwa", nowaNazwa);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas edytowania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas edytowania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Pobiera listę wszystkich lokalizacji dostępnych w bazie danych.
        /// </summary>
        /// <returns>Lista obiektów reprezentujących lokalizacje.</returns>
        public List<Lokalizacja> GetLokalizacje()
        {
            List<Lokalizacja> lokalizacje = new List<Lokalizacja>();

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, Nazwa, Adres, PojemnoscMaksymalna FROM Lokalizacje";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lokalizacje.Add(new Lokalizacja
                            {
                                ID = reader.GetInt32(0),
                                Nazwa = reader.GetString(1),
                                Adres = reader.GetString(2),
                                PojemnoscMaksymalna = reader.GetInt32(3)
                            });
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas pobierania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas pobierania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return lokalizacje;
        }

        /// <summary>
        /// Dodaje nową lokalizację do bazy danych.
        /// </summary>
        /// <param name="nazwa">Nazwa lokalizacji.</param>
        /// <param name="adres">Adres lokalizacji.</param>
        /// <param name="pojemnoscMaksymalna">Maksymalna pojemność lokalizacji.</param>
        public void DodajLokalizacja(string nazwa, string adres, int pojemnoscMaksymalna)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Lokalizacje (Nazwa, Adres, PojemnoscMaksymalna) VALUES (@nazwa, @adres, @pojemnosc)";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nazwa", nazwa);
                        cmd.Parameters.AddWithValue("@adres", adres);
                        cmd.Parameters.AddWithValue("@pojemnosc", pojemnoscMaksymalna);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas dodawania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas dodawania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        /// <summary>
        /// Edytuje dane istniejącej lokalizacji w bazie danych.
        /// </summary>
        /// <param name="id">ID edytowanej lokalizacji.</param>
        /// <param name="nowaNazwa">Nowa nazwa lokalizacji.</param>
        /// <param name="nowyAdres">Nowy adres lokalizacji.</param>
        /// <param name="nowaPojemnosc">Nowa maksymalna pojemność lokalizacji.</param>
        public void EdytujLokalizacja(int id, string nowaNazwa, string nowyAdres, int nowaPojemnosc)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Lokalizacje SET Nazwa = @nazwa, Adres = @adres, PojemnoscMaksymalna = @pojemnosc WHERE ID = @id";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@nazwa", nowaNazwa);
                        cmd.Parameters.AddWithValue("@adres", nowyAdres);
                        cmd.Parameters.AddWithValue("@pojemnosc", nowaPojemnosc);
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas edytowania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas edytowania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Usuwa lokalizację z bazy danych na podstawie jej ID.
        /// </summary>
        /// <param name="id">ID lokalizacji do usunięcia.</param>
        public void UsunLokalizacja(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Lokalizacje WHERE ID = @id";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas usuwania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas usuwania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Pobiera listę wszystkich administratorów z bazy danych.
        /// </summary>
        /// <returns>Lista obiektów reprezentujących administratorów.</returns>
        public List<Administrator> GetAdministratorzy()
        {
            var administratorzy = new List<Administrator>();

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT ID, ImieNazwisko, Email FROM Administratorzy";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            administratorzy.Add(new Administrator
                            {
                                ID = reader.GetInt32(0),
                                ImieNazwisko = reader.GetString(1),
                                Email = reader.GetString(2)
                            });
                        }
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas pobierania administratorów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas pobierania administratorów: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return administratorzy;
        }

        /// <summary>
        /// Dodaje nowego administratora do bazy danych.
        /// </summary>
        /// <param name="imieNazwisko">Imię i nazwisko administratora.</param>
        /// <param name="email">Adres e-mail administratora.</param>
        /// <param name="haslo">Hasło administratora.</param>
        public void DodajAdministrator(string imieNazwisko, string email, string haslo)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Administratorzy (ImieNazwisko, Email, Haslo) VALUES (@imieNazwisko, @email, @haslo)";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@imieNazwisko", imieNazwisko);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@haslo", haslo);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas dodawania administratora: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas dodawania administratora: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Usuwa administratora z bazy danych na podstawie jego ID.
        /// </summary>
        /// <param name="id">ID administratora do usunięcia.</param>
        public void UsunAdministrator(int id)
        {
            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Administratorzy WHERE ID = @id";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show($"Błąd podczas usuwania administratora: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Nieoczekiwany błąd podczas usuwania administratora: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

