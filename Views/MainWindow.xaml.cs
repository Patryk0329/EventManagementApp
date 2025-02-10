using System.Windows;
using EventManagementApp.DataAccess;

namespace EventManagementApp
{
    /// <summary>
    /// Główne okno aplikacji, umożliwiające nawigację do paneli uczestnika, organizatora i administratora.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="MainWindow"/>.
        /// </summary>
        public MainWindow()
        {
            //TestDatabaseConnection();
            InitializeComponent();
        }

        //private void TestDatabaseConnection()
        //{
        //    var db = new Database();
        //    bool isConnected = db.TestConnection();
        //    MessageBox.Show(isConnected ? "Połączenie z bazą danych powiodło się" : "Błąd połączenia");
        //}

        /// <summary>
        /// Obsługuje kliknięcie przycisku uczestnika, otwierając panel uczestnika.
        /// </summary>
        private void UczestnikButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            UczestnikPanel uczestnikPanel = new UczestnikPanel();
            uczestnikPanel.Owner = this;
            uczestnikPanel.Show();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku organizatora, otwierając okno organizatora.
        /// </summary>
        private void OrganizatorButton_Click(object sender, RoutedEventArgs e)
        {
            OrganizatorWindow organizatorWindow = new OrganizatorWindow();
            organizatorWindow.Owner = this;
            organizatorWindow.Show();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku administratora, otwierając okno logowania administratora.
        /// </summary>
        private void AdministratorButton_Click(object sender, RoutedEventArgs e)
        {
            AdminLoginWindow adminLoginWindow = new AdminLoginWindow();
            adminLoginWindow.Owner = this;
            adminLoginWindow.Show();
        }
    }
}