namespace EventManagementApp.DataAccess
{
    /// <summary>
    /// Reprezentuje kategorię wydarzenia w systemie zarządzania wydarzeniami.
    /// </summary>
    public class Kategoria
    {
        /// <summary>
        /// Unikalny identyfikator kategorii.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Nazwa kategorii.
        /// </summary>
        public string Nazwa { get; set; }
    }
}