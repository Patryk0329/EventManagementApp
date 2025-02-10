using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Główne okno administracyjne, umożliwiające dostęp do zarządzania kategoriami, lokalizacjami i administratorami.
    /// </summary>
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku kategorii, otwierając okno zarządzania kategoriami.
        /// </summary>
        private void KategorieButton_Click(object sender, RoutedEventArgs e)
        {
            KategorieWindow kategorieWindow = new KategorieWindow();
            kategorieWindow.Show();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku lokalizacji, otwierając okno zarządzania lokalizacjami.
        /// </summary>
        private void LokalizacjeButton_Click(object sender, RoutedEventArgs e)
        {
            LokalizacjeWindow lokalizacjeWindow = new LokalizacjeWindow();
            lokalizacjeWindow.Show();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku administratorów, otwierając okno zarządzania administratorami.
        /// </summary>
        private void AdministratorzyButton_Click(object sender, RoutedEventArgs e)
        {
            AdministratorzyWindow administratorzyWindow = new AdministratorzyWindow();
            administratorzyWindow.Show();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku powrotu, zamykając okno administracyjne.
        /// </summary>
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}