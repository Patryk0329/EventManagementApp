using EventManagementApp.DataAccess;
using System;
using System.Collections.Generic;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno rezerwacji, umożliwiające uczestnikom rezerwację miejsc na wydarzenia.
    /// </summary>
    public partial class RezerwacjaWindow : Window
    {
        private int uczestnikId;
        private List<Wydarzenie> wydarzenia;

        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="RezerwacjaWindow"/>.
        /// </summary>
        /// <param name="uczestnikId">Identyfikator uczestnika dokonującego rezerwacji.</param>
        public RezerwacjaWindow(int uczestnikId)
        {
            InitializeComponent();
            this.uczestnikId = uczestnikId;

            LoadWydarzenia();
        }

        /// <summary>
        /// Ładuje listę dostępnych wydarzeń z bazy danych.
        /// </summary>
        private void LoadWydarzenia()
        {
            try
            {
                var db = new Database();
                wydarzenia = db.GetWydarzenia();

                WydarzeniaListBox.ItemsSource = wydarzenia;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania wydarzeń: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku powrotu, zamykając bieżące okno i pokazując okno właściciela.
        /// </summary>
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku potwierdzenia, dokonując rezerwacji wybranego wydarzenia.
        /// </summary>
        private void Potwierdz_Click(object sender, RoutedEventArgs e)
        {
            if (WydarzeniaListBox.SelectedValue == null)
            {
                MessageBox.Show("Proszę wybrać wydarzenie!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                int wydarzenieId = (int)WydarzeniaListBox.SelectedValue;

                var db = new Database();
                bool result = db.ZarezerwujMiejsce(uczestnikId, wydarzenieId);

                if (result)
                {
                    MessageBox.Show("Rezerwacja została pomyślnie zapisana!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Owner.Show();
                    this.Close();
                }
                else
                {
                    // W przypadku błędu szczegółowy komunikat jest już wyświetlany w metodzie ZarezerwujMiejsce
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas rezerwacji: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}