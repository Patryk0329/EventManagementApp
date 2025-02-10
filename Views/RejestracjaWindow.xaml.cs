using System.Windows;
using EventManagementApp.DataAccess;

namespace EventManagementApp
{
    /// <summary>
    /// Okno rejestracji, umożliwiające nowym użytkownikom rejestrację w systemie.
    /// </summary>
    public partial class RejestracjaWindow : Window
    {
        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="RejestracjaWindow"/>.
        /// </summary>
        public RejestracjaWindow()
        {
            InitializeComponent();

            // Monitorowanie zmian w polu Imię i Nazwisko
            ImieNazwiskoTextBox.TextChanged += (sender, e) =>
            {
                ImieNazwiskoWatermark.Visibility = string.IsNullOrEmpty(ImieNazwiskoTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            };
            if (string.IsNullOrEmpty(ImieNazwiskoTextBox.Text))
            {
                ImieNazwiskoWatermark.Visibility = Visibility.Visible;
            }

            // Monitorowanie zmian w polu Email
            EmailTextBox.TextChanged += (sender, e) =>
            {
                EmailWatermark.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            };
            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                EmailWatermark.Visibility = Visibility.Visible;
            }

            // Monitorowanie zmian w polu Hasło
            HasloBox.PasswordChanged += (sender, e) =>
            {
                HasloWatermark.Visibility = string.IsNullOrEmpty(HasloBox.Password) ? Visibility.Visible : Visibility.Collapsed;
            };
            if (string.IsNullOrEmpty(HasloBox.Password))
            {
                HasloWatermark.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku rejestracji, rejestrując nowego użytkownika.
        /// </summary>
        private void Rejestracja_Click(object sender, RoutedEventArgs e)
        {
            string imieNazwisko = ImieNazwiskoTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string haslo = HasloBox.Password;

            if (string.IsNullOrWhiteSpace(imieNazwisko) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(haslo))
            {
                MessageBox.Show("Wszystkie pola są wymagane.");
                return;
            }

            var db = new Database();
            bool success = db.ZarejestrujUczestnika(imieNazwisko, email, haslo);

            if (success)
            {
                MessageBox.Show("Rejestracja zakończona sukcesem!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Wystąpił błąd podczas rejestracji.");
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku powrotu, zamykając okno rejestracji.
        /// </summary>
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}