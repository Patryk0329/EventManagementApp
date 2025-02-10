using EventManagementApp.DataAccess;
using System.Collections.ObjectModel;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno zarządzania lokalizacjami, umożliwiające dodawanie, edytowanie i usuwanie lokalizacji.
    /// </summary>
    public partial class LokalizacjeWindow : Window
    {
        private readonly Database db;
        public ObservableCollection<Lokalizacja> Lokalizacje { get; set; }

        /// <summary>
        /// Inicjalizuje nowe wystąpienie klasy <see cref="LokalizacjeWindow"/>.
        /// </summary>
        public LokalizacjeWindow()
        {
            InitializeComponent();
            db = new Database();
            Lokalizacje = new ObservableCollection<Lokalizacja>(db.GetLokalizacje());
            LokalizacjeListView.ItemsSource = Lokalizacje;
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku dodawania lokalizacji, otwierając okno dodawania lokalizacji.
        /// </summary>
        private void DodajButton_Click(object sender, RoutedEventArgs e)
        {
            var oknoDodaj = new DodajLokalizacjeWindow();
            if (oknoDodaj.ShowDialog() == true)
            {
                Lokalizacje.Clear();
                foreach (var lokalizacja in db.GetLokalizacje())
                {
                    Lokalizacje.Add(lokalizacja);
                }
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku edytowania lokalizacji, otwierając okno edytowania lokalizacji.
        /// </summary>
        private void EdytujButton_Click(object sender, RoutedEventArgs e)
        {
            if (LokalizacjeListView.SelectedItem is Lokalizacja selectedLokalizacja)
            {
                var oknoEdytuj = new EdytujLokalizacjaWindow(selectedLokalizacja);
                if (oknoEdytuj.ShowDialog() == true)
                {
                    Lokalizacje.Clear();
                    foreach (var lokalizacja in db.GetLokalizacje())
                    {
                        Lokalizacje.Add(lokalizacja);
                    }
                }
            }
            else
            {
                MessageBox.Show("Proszę wybrać lokalizację do edycji.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku usuwania lokalizacji, usuwając wybraną lokalizację.
        /// </summary>
        private void UsunButton_Click(object sender, RoutedEventArgs e)
        {
            if (LokalizacjeListView.SelectedItem is Lokalizacja selectedLokalizacja)
            {
                var result = MessageBox.Show($"Czy na pewno chcesz usunąć lokalizację \"{selectedLokalizacja.Nazwa}\"?",
                                             "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    db.UsunLokalizacja(selectedLokalizacja.ID);
                    Lokalizacje.Remove(selectedLokalizacja);
                }
            }
            else
            {
                MessageBox.Show("Proszę wybrać lokalizację do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku zamknięcia okna.
        /// </summary>
        private void ZamknijButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}