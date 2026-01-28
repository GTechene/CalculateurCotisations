namespace Cotisations;

// TODO : l'interface n'est utile que pour l'export Excel.
public interface IConstantesAvecHistorique
{
    public decimal CotisationsMaladiePourRevenusInferieursA60PctDuPass { get; }
    public decimal CotisationsMaladiePourRevenusSupA60PctPass { get; }
    public decimal CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass { get; }
    public decimal CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass { get; }
    public int PlafondsRetraiteComplementaireArtisansCommercants { get; }
}

public class Constantes2023 : IConstantesAvecHistorique
{
    public decimal CotisationsMaladiePourRevenusInferieursA60PctDuPass => 0.0365m;
    public decimal CotisationsMaladiePourRevenusSupA60PctPass => 0.0635m;
    public decimal CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass => 0.005m;
    public decimal CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass => 0.0085m; 
    public int PlafondsRetraiteComplementaireArtisansCommercants => 40784;
}

public class Constantes2024 : IConstantesAvecHistorique
{
    public decimal CotisationsMaladiePourRevenusInferieursA60PctDuPass => 0.04m;
    public decimal CotisationsMaladiePourRevenusSupA60PctPass => 0.067m;
    public decimal CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass => 0.005m;
    public decimal CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass => throw new InvalidOperationException("Ce taux n'existe pas pour l'année 2024");
    public int PlafondsRetraiteComplementaireArtisansCommercants => 42946;
    public decimal PlancherRetraite450HeuresDeSmic = 5242.5m;
}

public class Constantes2025
{
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass = 0.015m;
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass = 0.04m;
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass = 0.065m;
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass = 0.077m;
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass = 0.085m;
    public const decimal TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass = 0.065m;
    public const decimal TauxCotisationsIndemnitesMaladie = 0.005m;

    // Basé sur 450h de SMIC (SMIC horaire = 11.88 € en 2025)
    public const int RevenusPlancherRetraiteDeBase = 5346;

    public const decimal TauxCotisationsRetraiteBaseRevenusSuperieursAuPass = 0.0072m;
    public const decimal TauxCotisationsRetraiteBaseRevenusInferieursAuPass = 0.1787m;
    public const decimal TauxRetraiteComplementairePremiereTrancheArtisansCommercants = 0.081m;
    public const decimal TauxRetraiteComplementaireDeuxiemeTrancheArtisansCommercants = 0.091m;
}

public class Constantes2026
{
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass = 0.015m;
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass = 0.04m;
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass = 0.065m;
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass = 0.077m;
    public const decimal TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass = 0.085m;
    public const decimal TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass = 0.065m;
    public const decimal TauxCotisationsIndemnitesMaladie = 0.005m;

    // Basé sur 450h de SMIC (SMIC horaire = 12.02 € en 2026)
    public const int RevenusPlancherRetraiteDeBase = 5409;

    public const decimal TauxCotisationsRetraiteBaseRevenusSuperieursAuPass = 0.0072m;
    public const decimal TauxCotisationsRetraiteBaseRevenusInferieursAuPass = 0.1787m;
    public const decimal TauxRetraiteComplementairePremiereTrancheArtisansCommercants = 0.081m;
    public const decimal TauxRetraiteComplementaireDeuxiemeTrancheArtisansCommercants = 0.091m;
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