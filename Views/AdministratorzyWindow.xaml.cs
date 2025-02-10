using EventManagementApp.DataAccess;
using EventManagementApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Okno zarządzania administratorami, umożliwiające dodawanie i usuwanie administratorów.
    /// </summary>
    public partial class AdministratorzyWindow : Window
    {
        private readonly Database db = new Database();
        public ObservableCollection<Administrator> Administratorzy { get; set; }

        public AdministratorzyWindow()
        {
            InitializeComponent();
            Administratorzy = new ObservableCollection<Administrator>(db.GetAdministratorzy());
            AdministratorzyListView.ItemsSource = Administratorzy;
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku dodawania, otwierając okno dodawania administratora.
        /// </summary>
        private void DodajButton_Click(object sender, RoutedEventArgs e)
        {
            var dodajOkno = new DodajAdministratoraWindow();
            if (dodajOkno.ShowDialog() == true)
            {
                Administratorzy.Clear();
                foreach (var admin in db.GetAdministratorzy())
                {
                    Administratorzy.Add(admin);
                }
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku usuwania, usuwając wybranego administratora.
        /// </summary>
        private void UsunButton_Click(object sender, RoutedEventArgs e)
        {
            if (AdministratorzyListView.SelectedItem is Administrator wybranyAdmin)
            {
                if (MessageBox.Show($"Czy na pewno chcesz usunąć administratora {wybranyAdmin.ImieNazwisko}?", "Potwierdzenie", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    db.UsunAdministrator(wybranyAdmin.ID);
                    Administratorzy.Remove(wybranyAdmin);
                }
            }
            else
            {
                MessageBox.Show("Proszę wybrać administratora do usunięcia.", "Błąd", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Obsługuje kliknięcie przycisku zamknięcia, zamykając okno zarządzania administratorami.
        /// </summary>
        private void ZamknijButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}