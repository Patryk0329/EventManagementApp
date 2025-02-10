using EventManagementApp.DataAccess;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno edycji lokalizacji, umożliwiające użytkownikom edytowanie szczegółów lokalizacji.
    /// </summary>
    public partial class EdytujLokalizacjaWindow : Window
    {
        private readonly Lokalizacja lokalizacja;

        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="EdytujLokalizacjaWindow"/>.
        /// </summary>
        /// <param name="lokalizacja">Lokalizacja do edycji.</param>
        public EdytujLokalizacjaWindow(Lokalizacja lokalizacja)
        {
            InitializeComponent();
            this.lokalizacja = lokalizacja;

            NazwaTextBox.Text = lokalizacja.Nazwa;
            AdresTextBox.Text = lokalizacja.Adres;
            PojemnoscTextBox.Text = lokalizacja.PojemnoscMaksymalna.ToString();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku zapisu, aktualizując lokalizację w bazie danych.
        /// </summary>
        private void ZapiszButton_Click(object sender, RoutedEventArgs e)
        {
            string nowaNazwa = NazwaTextBox.Text.Trim();
            string nowyAdres = AdresTextBox.Text.Trim();
            if (!int.TryParse(PojemnoscTextBox.Text.Trim(), out int nowaPojemnosc) || nowaPojemnosc <= 0)
            {
                MessageBox.Show("Proszę podać poprawną maksymalną pojemność.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(nowaNazwa) || string.IsNullOrEmpty(nowyAdres))
            {
                MessageBox.Show("Proszę uzupełnić wszystkie pola.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var db = new Database();
                db.EdytujLokalizacja(lokalizacja.ID, nowaNazwa, nowyAdres, nowaPojemnosc);
                MessageBox.Show("Lokalizacja została zaktualizowana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Błąd podczas edytowania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania, zamykając okno edycji lokalizacji.
        /// </summary>
        private void AnulujButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}