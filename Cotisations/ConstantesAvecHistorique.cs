namespace Cotisations;

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
    public int PlafondsRetraiteComplementaireArtisansCommercants => 42946;
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