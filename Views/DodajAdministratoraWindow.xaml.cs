using EventManagementApp.DataAccess;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno dodawania administratora, umożliwiające użytkownikom dodawanie nowych administratorów.
    /// </summary>
    public partial class DodajAdministratoraWindow : Window
    {
        public DodajAdministratoraWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku zapisu, dodając nowego administratora do bazy danych.
        /// </summary>
        private void ZapiszButton_Click(object sender, RoutedEventArgs e)
        {
            string imieNazwisko = ImieNazwiskoTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string haslo = HasloPasswordBox.Password.Trim();

            if (string.IsNullOrEmpty(imieNazwisko) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(haslo))
            {
                MessageBox.Show("Proszę uzupełnić wszystkie pola.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var db = new Database();
                db.DodajAdministrator(imieNazwisko, email, haslo);
                MessageBox.Show("Administrator został dodany.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania administratora: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania, zamykając okno dodawania administratora.
        /// </summary>
        private void AnulujButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}