using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Panel uczestnika, umożliwiający dostęp do różnych funkcji systemu.
    /// </summary>
    public partial class UczestnikPanel : Window
    {
        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="UczestnikPanel"/>.
        /// </summary>
        public UczestnikPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku wydarzeń, otwierając okno z listą wydarzeń.
        /// </summary>
        private void Wydarzenia_Click(object sender, RoutedEventArgs e)
        {
            WydarzeniaWindow wydarzeniaWindow = new WydarzeniaWindow();
            wydarzeniaWindow.Owner = this;
            wydarzeniaWindow.ShowDialog();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku powrotu, zamykając panel uczestnika i powracając do głównego okna.
        /// </summary>
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku rejestracji, otwierając okno rejestracji.
        /// </summary>
        private void Rejestracja_Click(object sender, RoutedEventArgs e)
        {
            RejestracjaWindow rejestracjaWindow = new RejestracjaWindow();
            rejestracjaWindow.ShowDialog();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku logowania, otwierając okno logowania.
        /// </summary>
        private void Logowanie_Click(object sender, RoutedEventArgs e)
        {
            LogowanieWindow logowanieWindow = new LogowanieWindow();
            logowanieWindow.ShowDialog();
        }
    }
}