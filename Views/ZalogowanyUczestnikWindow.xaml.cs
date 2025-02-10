using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno dla zalogowanego uczestnika, umożliwiające dostęp do różnych funkcji systemu.
    /// </summary>
    public partial class ZalogowanyUczestnikWindow : Window
    {
        private int uczestnikId;

        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="ZalogowanyUczestnikWindow"/>.
        /// </summary>
        /// <param name="uczestnikId">Identyfikator zalogowanego uczestnika.</param>
        public ZalogowanyUczestnikWindow(int uczestnikId)
        {
            InitializeComponent();
            this.uczestnikId = uczestnikId;
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku rezerwacji, otwierając okno rezerwacji.
        /// </summary>
        private void Rezerwacja_Click(object sender, RoutedEventArgs e)
        {
            var rezerwacjaWindow = new RezerwacjaWindow(uczestnikId)
            {
                Owner = this
            };
            this.Hide();
            rezerwacjaWindow.ShowDialog();
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
        /// Obsługuje kliknięcie przycisku raportów, otwierając okno raportów.
        /// </summary>
        private void Raporty_Click(object sender, RoutedEventArgs e)
        {
            var raportyWindow = new RaportyWindow(uczestnikId)
            {
                Owner = this
            };
            this.Hide();
            raportyWindow.ShowDialog();
            this.Show();
        }
    }
}