using EventManagementApp.DataAccess;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno dodawania kategorii, umożliwiające użytkownikom dodawanie nowych kategorii.
    /// </summary>
    public partial class DodajKategoriaWindow : Window
    {
        public DodajKategoriaWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku dodawania, dodając nową kategorię do bazy danych.
        /// </summary>
        private void DodajButton_Click(object sender, RoutedEventArgs e)
        {
            string nazwa = NazwaTextBox.Text.Trim();

            if (string.IsNullOrEmpty(nazwa))
            {
                MessageBox.Show("Proszę podać nazwę kategorii.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var db = new Database();
                db.DodajKategoria(nazwa);
                MessageBox.Show("Kategoria została dodana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania, zamykając okno dodawania kategorii.
        /// </summary>
        private void AnulujButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}