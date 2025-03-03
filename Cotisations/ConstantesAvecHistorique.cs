namespace Cotisations;

// TODO : l'interface n'est utile que pour l'export Excel.
public interface IConstantesAvecHistorique
{
    public decimal CotisationsMaladiePourRevenusSupA60PctPass { get; }
    public decimal CotisationsMaladiePourRevenusSupA5Pass { get; }
    public decimal CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass { get; }
    public decimal CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass { get; }
    public int PlafondsRetraiteComplementaireArtisansCommercants { get; }
}

public class Constantes2023 : IConstantesAvecHistorique
{
    public decimal CotisationsMaladiePourRevenusSupA60PctPass => 0.0635m;
    public decimal CotisationsMaladiePourRevenusSupA5Pass => 0.065m;
    public decimal CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass => 0.005m;
    public decimal CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass => 0.0085m; 
    public int PlafondsRetraiteComplementaireArtisansCommercants => 40784;
}

public class Constantes2024 : IConstantesAvecHistorique
{
    public decimal CotisationsMaladiePourRevenusSupA60PctPass => 0.067m;
    public decimal CotisationsMaladiePourRevenusSupA5Pass => 0.065m;
    public decimal CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass => 0.005m;
    public decimal CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass => throw new InvalidOperationException("Ce taux n'existe pas pour l'année 2024");
    public int PlafondsRetraiteComplementaireArtisansCommercants => 42946;
    public decimal PlancherRetraite450HeuresDeSmic = 5242.5m;
}

public class Constantes2025
{
    public decimal TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass => 0.015m;
    public decimal TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass => 0.04m;
    public decimal TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass => 0.065m;
    public decimal TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass => 0.077m;
    public decimal TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass => 0.085m;
    public decimal TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass => 0.065m;
    public decimal TauxCotisationsIndemnitesMaladie => 0.005m;

    // TODO : va probablement changer en 2025 mais pas défini encore au 24/01/2025. Normalement c'est le PMSS * 0.9973 (pro rata temporis, donc 1 année complète - le 1er mai) * 12 mois. cf https://mon-entreprise.urssaf.fr/documentation/entreprise/prorata-temporis
    public int PlafondsRetraiteComplementaireArtisansCommercants => 42946;

    public decimal TauxCotisationsRetraiteBaseRevenusSuperieursAuPass = 0.072m;
    public decimal TauxCotisationsRetraiteBaseRevenusInferieursAuPass = 0.1787m;
    public decimal TauxRetraiteComplementairePremiereTrancheArtisansCommercants = 0.081m;
    public decimal TauxRetraiteComplementaireDeuxiemeTrancheArtisansCommercants = 0.091m;
}

public static class ConstantesAvecHistorique
{
    public static IConstantesAvecHistorique PourLAnnee(int annee)
    {
        return annee switch
        {
            2023 => new Constantes2023(),
            2024 => new Constantes2024(),
            _ => new Constantes2024()
        };
    }
}