using EventManagementApp.DataAccess;
using System;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno dodawania lokalizacji, umożliwiające użytkownikom dodawanie nowych lokalizacji.
    /// </summary>
    public partial class DodajLokalizacjeWindow : Window
    {
        public DodajLokalizacjeWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku zapisu, dodając nową lokalizację do bazy danych.
        /// </summary>
        private void ZapiszButton_Click(object sender, RoutedEventArgs e)
        {
            string nazwa = NazwaTextBox.Text.Trim();
            string adres = AdresTextBox.Text.Trim();
            string pojemnoscStr = PojemnoscTextBox.Text.Trim();

            if (string.IsNullOrEmpty(nazwa) || string.IsNullOrEmpty(adres) || string.IsNullOrEmpty(pojemnoscStr))
            {
                MessageBox.Show("Proszę uzupełnić wszystkie pola.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(pojemnoscStr, out int pojemnosc) || pojemnosc <= 0)
            {
                MessageBox.Show("Pojemność musi być liczbą całkowitą większą od zera.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var db = new Database();
                db.DodajLokalizacja(nazwa, adres, pojemnosc);
                MessageBox.Show("Lokalizacja została dodana.", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas dodawania lokalizacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku anulowania, zamykając okno dodawania lokalizacji.
        /// </summary>
        private void AnulujButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}