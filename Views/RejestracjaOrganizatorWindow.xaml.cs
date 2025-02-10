using EventManagementApp.DataAccess;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno rejestracji organizatora, umożliwiające rejestrację nowych organizatorów wydarzeń.
    /// </summary>
    public partial class RejestracjaOrganizatoraWindow : Window
    {
        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="RejestracjaOrganizatoraWindow"/>.
        /// </summary>
        public RejestracjaOrganizatoraWindow()
        {
            InitializeComponent();

            // Obsługa widoczności watermarków
            NazwaFirmyTextBox.TextChanged += (s, e) =>
            {
                NazwaFirmyWatermark.Visibility = string.IsNullOrEmpty(NazwaFirmyTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            };
            KontaktTextBox.TextChanged += (s, e) =>
            {
                KontaktWatermark.Visibility = string.IsNullOrEmpty(KontaktTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            };
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku rejestracji, rejestrując nowego organizatora.
        /// </summary>
        private void ZarejestrujButton_Click(object sender, RoutedEventArgs e)
        {
            string nazwaFirmy = NazwaFirmyTextBox.Text;
            string kontakt = KontaktTextBox.Text;

            if (string.IsNullOrWhiteSpace(nazwaFirmy) || string.IsNullOrWhiteSpace(kontakt))
            {
                MessageBox.Show("Proszę wypełnić wszystkie pola!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var db = new Database();
                bool result = db.DodajOrganizatora(nazwaFirmy, kontakt);

                if (result)
                {
                    MessageBox.Show("Organizator został pomyślnie zarejestrowany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Wystąpił problem podczas rejestracji organizatora.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Błąd podczas rejestracji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku powrotu, zamykając okno rejestracji organizatora.
        /// </summary>
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}