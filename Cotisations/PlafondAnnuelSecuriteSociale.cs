namespace Cotisations;

public class PlafondAnnuelSecuriteSociale(int year)
{
    private static readonly Dictionary<int, decimal> HistoriquePASS = new()
    {
        { 2023, 43992m },
        { 2024, 46368m },
        { 2025, 47100m },
        { 2026, 48060m },
    };
    
    public decimal Valeur { init; get; } = HistoriquePASS[year];

    public decimal Valeur40Pct => 0.4m * Valeur;
    public decimal Valeur60Pct => 0.6m * Valeur;
    public decimal Valeur110Pct => 1.1m * Valeur;
    public decimal Valeur140Pct => 1.4m * Valeur;
    public decimal Valeur400Pct => 4m * Valeur;
    public decimal Valeur500Pct => 5m * Valeur;
}