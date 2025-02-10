using System;
using System.Windows;

namespace EventManagementApp
{
    /// <summary>
    /// Główna klasa aplikacji, odpowiedzialna za uruchomienie aplikacji WPF.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Punkt wejścia aplikacji.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();
        }
    }
}