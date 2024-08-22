namespace CalculCotisations;

public class PlafondAnnuelSecuriteSociale
{
    public static readonly Dictionary<int, decimal> HistoriquePASS = new()
    {
        { 2023, 43992m },
        { 2024, 46368m }
    };
    
    public static PlafondAnnuelSecuriteSociale PASS => new(HistoriquePASS.Last().Key);

    public decimal Valeur { init; get; }

    public PlafondAnnuelSecuriteSociale(int year)
    {
        Valeur = HistoriquePASS[year];
    }

    public decimal Valeur40Pct => 0.4m * Valeur;
    public decimal Valeur60Pct => 0.6m * Valeur;
    public decimal Valeur110Pct => 1.1m * Valeur;
    public decimal Valeur140Pct => 1.4m * Valeur;
    public decimal Valeur500Pct => 5m * Valeur;
}