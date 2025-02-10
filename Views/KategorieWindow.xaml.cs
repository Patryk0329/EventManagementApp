using EventManagementApp.DataAccess;
using System.Collections.Generic;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno zarządzania kategoriami, umożliwiające dodawanie, edytowanie i usuwanie kategorii.
    /// </summary>
    public partial class KategorieWindow : Window
    {
        private List<Kategoria> kategorie;

        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="KategorieWindow"/>.
        /// </summary>
        public KategorieWindow()
        {
            InitializeComponent();
            LoadKategorie();
        }

        /// <summary>
        /// Ładuje listę kategorii z bazy danych i ustawia ją jako źródło danych dla DataGrid.
        /// </summary>
        private void LoadKategorie()
        {
            try
            {
                var db = new Database();
                kategorie = db.GetKategorie();
                KategorieDataGrid.ItemsSource = kategorie;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Błąd podczas ładowania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku dodawania kategorii, otwierając okno dodawania kategorii.
        /// </summary>
        private void DodajButton_Click(object sender, RoutedEventArgs e)
        {
            var oknoDodaj = new DodajKategoriaWindow();
            oknoDodaj.ShowDialog();
            LoadKategorie();
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku edytowania kategorii, otwierając okno edytowania kategorii.
        /// </summary>
        private void EdytujButton_Click(object sender, RoutedEventArgs e)
        {
            if (KategorieDataGrid.SelectedItem is Kategoria wybranaKategoria)
            {
                var oknoEdytuj = new EdytujKategoriaWindow(wybranaKategoria);
                oknoEdytuj.ShowDialog();
                LoadKategorie();
            }
            else
            {
                MessageBox.Show("Proszę wybrać kategorię do edycji.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku usuwania kategorii, usuwając wybraną kategorię.
        /// </summary>
        private void UsunButton_Click(object sender, RoutedEventArgs e)
        {
            if (KategorieDataGrid.SelectedItem is Kategoria wybranaKategoria)
            {
                var wynik = MessageBox.Show($"Czy na pewno chcesz usunąć kategorię '{wybranaKategoria.Nazwa}'?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (wynik == MessageBoxResult.Yes)
                {
                    try
                    {
                        var db = new Database();
                        db.UsunKategoria(wybranaKategoria.ID);
                        LoadKategorie();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"Błąd podczas usuwania kategorii: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Proszę wybrać kategorię do usunięcia.", "Informacja", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku powrotu, zamykając okno zarządzania kategoriami.
        /// </summary>
        private void PowrotButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}