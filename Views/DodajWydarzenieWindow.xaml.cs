using System;
using System.Linq;
using System.Windows;
using EventManagementApp.DataAccess;

namespace EventManagementApp
{
    /// <summary>
    /// Okno dodawania wydarzenia, umożliwiające użytkownikom dodawanie nowych wydarzeń.
    /// </summary>
    public partial class DodajWydarzenieWindow : Window
    {
        public DodajWydarzenieWindow()
        {
            InitializeComponent();

            // Placeholdery dla TextBoxów
            NazwaFirmyTextBox.TextChanged += (sender, e) =>
            {
                NazwaFirmyWatermark.Visibility = string.IsNullOrEmpty(NazwaFirmyTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            };

            KontaktTextBox.TextChanged += (sender, e) =>
            {
                KontaktWatermark.Visibility = string.IsNullOrEmpty(KontaktTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            };

            NazwaWydarzeniaTextBox.TextChanged += (sender, e) =>
            {
                NazwaWydarzeniaWatermark.Visibility = string.IsNullOrEmpty(NazwaWydarzeniaTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            };

            GodzinaTextBox.TextChanged += (sender, e) =>
            {
                GodzinaWatermark.Visibility = string.IsNullOrEmpty(GodzinaTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            };

            LiczbaMiejscTextBox.TextChanged += (sender, e) =>
            {
                LiczbaMiejscWatermark.Visibility = string.IsNullOrEmpty(LiczbaMiejscTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            };

            // Załaduj lokalizacje i kategorie z bazy danych
            LoadLokalizacje();
            LoadKategorie();
        }

        /// <summary>
        /// Ładuje lokalizacje z bazy danych i ustawia je jako źródło danych dla ComboBoxa.
        /// </summary>
        private void LoadLokalizacje()
        {
            Database db = new Database();
            var lokalizacje = db.GetLokalizacje(); // Pobierz lokalizacje z bazy
            LokalizacjaComboBox.ItemsSource = lokalizacje;
            LokalizacjaComboBox.DisplayMemberPath = "Nazwa";
        }

        /// <summary>
        /// Ładuje kategorie z bazy danych i ustawia je jako źródło danych dla ListBoxa.
        /// </summary>
        private void LoadKategorie()
        {
            Database db = new Database();
            var kategorie = db.GetKategorie(); // Pobierz kategorie z bazy
            KategorieListBox.ItemsSource = kategorie;
            KategorieListBox.DisplayMemberPath = "Nazwa";
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku dodawania wydarzenia, dodając nowe wydarzenie do bazy danych.
        /// </summary>
        private void DodajWydarzenie_Click(object sender, RoutedEventArgs e)
        {
            string nazwaFirmy = NazwaFirmyTextBox.Text;
            string kontakt = KontaktTextBox.Text;
            string nazwaWydarzenia = NazwaWydarzeniaTextBox.Text;
            DateTime? data = DataPicker.SelectedDate;
            string godzina = GodzinaTextBox.Text;
            var lokalizacja = LokalizacjaComboBox.SelectedItem;

            if (string.IsNullOrWhiteSpace(nazwaFirmy) || string.IsNullOrWhiteSpace(kontakt) ||
                string.IsNullOrWhiteSpace(nazwaWydarzenia) || !data.HasValue ||
                lokalizacja == null || !int.TryParse(LiczbaMiejscTextBox.Text, out int liczbaMiejsc))
            {
                MessageBox.Show("Wszystkie pola muszą być poprawnie wypełnione!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Database db = new Database();
            if (!db.SprawdzDaneFirmy(nazwaFirmy, kontakt))
            {
                MessageBox.Show("Firma o podanych danych nie istnieje w bazie!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Dodawanie wydarzenia
            db.DodajWydarzenie(nazwaFirmy, kontakt, nazwaWydarzenia, data.Value, godzina, ((Lokalizacja)lokalizacja).Nazwa, liczbaMiejsc, KategorieListBox.SelectedItems.Cast<Kategoria>().ToList());

        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania, zamykając okno dodawania wydarzenia.
        /// </summary>
        private void Anuluj_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}