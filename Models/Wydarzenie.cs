using System;

/// <summary>
/// Reprezentuje wydarzenie w systemie zarządzania wydarzeniami.
/// </summary>
public class Wydarzenie
{
    /// <summary>
    /// Unikalny identyfikator wydarzenia.
    /// </summary>
    public int ID { get; set; }

    /// <summary>
    /// Nazwa wydarzenia.
    /// </summary>
    public string Nazwa { get; set; }

    /// <summary>
    /// Data wydarzenia.
    /// </summary>
    public DateTime Data { get; set; }

    /// <summary>
    /// Godzina rozpoczęcia wydarzenia.
    /// </summary>
    public TimeSpan Godzina { get; set; }

    /// <summary>
    /// Nazwa lokalizacji, w której odbędzie się wydarzenie.
    /// </summary>
    public string LokalizacjaNazwa { get; set; }

    /// <summary>
    /// Liczba miejsc dostępnych na wydarzeniu.
    /// </summary>
    public int LiczbaMiejsc { get; set; }

    /// <summary>
    /// Nazwa organizatora wydarzenia.
    /// </summary>
    public string OrganizatorNazwa { get; set; }

    /// <summary>
    /// Kontakt do organizatora wydarzenia.
    /// </summary>
    public string OrganizatorKontakt { get; set; }

    /// <summary>
    /// Nazwa kategorii, do której należy wydarzenie.
    /// </summary>
    public string KategoriaNazwa { get; set; }
}