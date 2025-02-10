using EventManagementApp.DataAccess;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno edycji kategorii, umożliwiające użytkownikom edytowanie szczegółów kategorii.
    /// </summary>
    public partial class EdytujKategoriaWindow : Window
    {
        private readonly Kategoria kategoria;

        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="EdytujKategoriaWindow"/>.
        /// </summary>
        /// <param name="kategoria">Kategoria do edycji.</param>
        public EdytujKategoriaWindow(Kategoria kategoria)
        {
            InitializeComponent();
            this.kategoria = kategoria;
            NazwaTextBox.Text = kategoria.Nazwa;
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku zapisu, aktualizując kategorię w bazie danych.
        /// </summary>
        private void ZapiszButton_Click(object sender, RoutedEventArgs e)
        {
            string nowaNazwa = NazwaTextBox.Text.Trim();

            if (string.IsNullOrEmpty(nowaNazwa))
            {
                MessageBox.Show("Proszę podać nazwę kategorii.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var db = new Database();
                db.EdytujKategoria(kategoria.ID, nowaNazwa);
                MessageBox.Show("Kategoria została zaktualizowana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Błąd podczas edytowania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania, zamykając okno edycji kategorii.
        /// </summary>
        private void AnulujButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}