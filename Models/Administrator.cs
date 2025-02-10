namespace EventManagementApp.Models
{
    /// <summary>
    /// Reprezentuje administratora w systemie zarządzania wydarzeniami.
    /// </summary>
    public class Administrator
    {
        /// <summary>
        /// Unikalny identyfikator administratora.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Pełne imię i nazwisko administratora.
        /// </summary>
        public string ImieNazwisko { get; set; }

        /// <summary>
        /// Email administratora.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Hasło administratora (opcjonalne w modelu, zależnie od użycia).
        /// </summary>
        public string Haslo { get; set; }
    }
}