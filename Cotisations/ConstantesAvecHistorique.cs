namespace Cotisations;

public interface IConstantesAvecHistorique
{
    public decimal CotisationsMaladiePourRevenusSupA60PctPass { get; }
    public decimal CotisationsMaladiePourRevenusSupA5Pass { get; }
    public decimal CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass { get; }
    public decimal CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass { get; }
    public int PlafondsRetraiteComplementaireArtisansCommercants { get; }
}

public class Constantes2024 : IConstantesAvecHistorique
{
    public decimal CotisationsMaladiePourRevenusSupA60PctPass => 0.067m;
    public decimal CotisationsMaladiePourRevenusSupA5Pass => 0.065m;
    public decimal CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass => 0.005m;
    public decimal CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass => throw new InvalidOperationException("Ce taux n'existe pas pour l'année 2024");
    public int PlafondsRetraiteComplementaireArtisansCommercants => 42946;
}

public class Constantes2023 : IConstantesAvecHistorique
{
    public decimal CotisationsMaladiePourRevenusSupA60PctPass => 0.0635m;
    public decimal CotisationsMaladiePourRevenusSupA5Pass => 0.065m;
    public decimal CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass => 0.005m;
    public decimal CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass => 0.0085m; 
    public int PlafondsRetraiteComplementaireArtisansCommercants => 40784;
}