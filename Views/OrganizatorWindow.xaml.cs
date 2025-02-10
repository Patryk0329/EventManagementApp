using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno organizatora, umożliwiające zarządzanie wydarzeniami i rejestrację organizatorów.
    /// </summary>
    public partial class OrganizatorWindow : Window
    {
        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="OrganizatorWindow"/>.
        /// </summary>
        public OrganizatorWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku dodawania wydarzenia, otwierając okno dodawania wydarzenia.
        /// </summary>
        private void DodajWydarzenieButton_Click(object sender, RoutedEventArgs e)
        {
            var dodajWydarzenieWindow = new DodajWydarzenieWindow();
            dodajWydarzenieWindow.Owner = this;
            dodajWydarzenieWindow.ShowDialog();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku rejestracji organizatora, otwierając okno rejestracji organizatora.
        /// </summary>
        private void RejestracjaOrganizatoraButton_Click(object sender, RoutedEventArgs e)
        {
            var rejestracjaWindow = new RejestracjaOrganizatoraWindow();
            rejestracjaWindow.Owner = this;
            rejestracjaWindow.ShowDialog();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku powrotu, zamykając okno organizatora i pokazując okno właściciela.
        /// </summary>
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}