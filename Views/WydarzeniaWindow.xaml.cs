using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using EventManagementApp.DataAccess;

namespace EventManagementApp
{
    /// <summary>
    /// Okno wyświetlające listę wydarzeń oraz umożliwiające filtrowanie według kategorii.
    /// </summary>
    public partial class WydarzeniaWindow : Window
    {
        private Database _database;

        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="WydarzeniaWindow"/>.
        /// </summary>
        public WydarzeniaWindow()
        {
            InitializeComponent();
            _database = new Database();
            LoadKategorie();
            LoadWydarzenia();
        }

        /// <summary>
        /// Ładuje listę kategorii z bazy danych i ustawia je jako źródło danych dla ComboBoxa.
        /// </summary>
        private void LoadKategorie()
        {
            try
            {
                var kategorie = _database.GetKategorie();
                kategorie.Insert(0, new Kategoria { ID = 0, Nazwa = "Wszystkie" });
                KategoriaComboBox.ItemsSource = kategorie;
                KategoriaComboBox.SelectedIndex = 0;
            }
            catch
            {
                MessageBox.Show("Nie udało się załadować kategorii.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Ładuje listę wydarzeń z bazy danych i ustawia je jako źródło danych dla DataGrid.
        /// </summary>
        private void LoadWydarzenia()
        {
            try
            {
                List<Wydarzenie> wydarzenia = _database.GetWydarzenia();
                WydarzeniaDataGrid.ItemsSource = wydarzenia;
            }
            catch
            {
                MessageBox.Show("Nie udało się załadować wydarzeń. Spróbuj ponownie później.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku powrotu, zamykając okno.
        /// </summary>
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Obsługuje zmianę wybranej kategorii w ComboBoxie, filtrując listę wydarzeń.
        /// </summary>
        private void KategoriaComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedKategoriaId = (int)(KategoriaComboBox.SelectedValue ?? 0);

            if (selectedKategoriaId == 0)
            {
                LoadWydarzenia();
            }
            else
            {
                var wydarzenia = _database.GetWydarzeniaByKategoria(selectedKategoriaId);
                WydarzeniaDataGrid.ItemsSource = wydarzenia;
            }
        }
    }
}