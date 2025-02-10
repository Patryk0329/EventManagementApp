using System.Windows;
using EventManagementApp.DataAccess;

namespace EventManagementApp
{
    /// <summary>
    /// Okno logowania, umożliwiające użytkownikom logowanie się do systemu.
    /// </summary>
    public partial class LogowanieWindow : Window
    {
        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="LogowanieWindow"/>.
        /// </summary>
        public LogowanieWindow()
        {
            InitializeComponent();

            EmailTextBox.TextChanged += (sender, e) =>
            {
                EmailWatermark.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Collapsed;
            };
            if (string.IsNullOrEmpty(EmailTextBox.Text))
            {
                EmailWatermark.Visibility = Visibility.Visible;
            }

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
        /// Obsługuje kliknięcie przycisku logowania, próbując zalogować użytkownika.
        /// </summary>
        private void Logowanie_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string haslo = HasloBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(haslo))
            {
                MessageBox.Show("Email i hasło nie mogą być puste!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Database db = new Database();
            if (db.ZalogujUczestnika(email, haslo))
            {
                int uczestnikId = db.GetUczestnikId(email);
                MessageBox.Show("Logowanie powiodło się!");
                ZalogowanyUczestnikWindow panel = new ZalogowanyUczestnikWindow(uczestnikId);
                panel.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Nieprawidłowy email lub hasło.");
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku powrotu, zamykając okno logowania.
        /// </summary>
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}