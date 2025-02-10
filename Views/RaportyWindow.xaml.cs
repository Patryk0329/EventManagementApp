using System;
using System.Collections.Generic;
using System.Windows;
using EventManagementApp.DataAccess;

namespace EventManagementApp
{
    /// <summary>
    /// Okno raportów, umożliwiające przeglądanie różnych raportów związanych z wydarzeniami.
    /// </summary>
    public partial class RaportyWindow : Window
    {
        private int uczestnikId;

        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="RaportyWindow"/>.
        /// </summary>
        /// <param name="uczestnikId">Identyfikator uczestnika.</param>
        public RaportyWindow(int uczestnikId)
        {
            InitializeComponent();
            this.uczestnikId = uczestnikId;
        }

        /// <summary>
        /// Obsługuje zmianę wyboru w ComboBoxie raportów, ładując odpowiedni raport.
        /// </summary>
        private void RaportyComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (RaportyComboBox.SelectedIndex == 0)
            {
                LoadWydarzeniaUczestnika();
            }
            else if (RaportyComboBox.SelectedIndex == 1)
            {
                LoadNajpopularniejszeWydarzenia();
            }
            else if (RaportyComboBox.SelectedIndex == 2)
            {
                LoadWolneMiejscaWydarzenia();
            }
        }

        /// <summary>
        /// Ładuje raport wydarzeń uczestnika.
        /// </summary>
        private void LoadWydarzeniaUczestnika()
        {
            try
            {
                var db = new Database();
                var data = db.GetWydarzeniaUczestnika(uczestnikId);
                RaportyDataGrid.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania raportu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ładuje raport najpopularniejszych wydarzeń.
        /// </summary>
        private void LoadNajpopularniejszeWydarzenia()
        {
            try
            {
                var db = new Database();
                var data = db.GetNajpopularniejszeWydarzenia();
                RaportyDataGrid.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania raportu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ładuje raport wolnych miejsc na wydarzeniach.
        /// </summary>
        private void LoadWolneMiejscaWydarzenia()
        {
            try
            {
                var db = new Database();
                var data = db.GetWolneMiejscaWydarzenia();
                RaportyDataGrid.ItemsSource = data;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania raportu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}