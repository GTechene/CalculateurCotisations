using System.Globalization;

namespace Cotisations;

public class Calculateur2024 : ICalculateur
{
    private readonly PlafondAnnuelSecuriteSociale PASS;
    private readonly Constantes2024 _constantes;
    private readonly CalculateurCommun _calculateurCommun;

    public Calculateur2024()
    {
        PASS = new PlafondAnnuelSecuriteSociale(2024);
        _constantes = new Constantes2024();
        _calculateurCommun = new CalculateurCommun(PASS);
    }

    public void CalculeLesCotisations(decimal revenu)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

        AssietteDeCalculDesCotisations = revenu;
        MaladieHorsIndemnitesJournalieres = _calculateurCommun.CalculeLesCotisationsMaladieHorsIndemnitesJournalieresAvant2025(revenu, _constantes.CotisationsMaladiePourRevenusSupA60PctPass, _constantes.CotisationsMaladiePourRevenusSupA5Pass);
        CalculeLesCotisationsPourIndemnitesMaladie(revenu);
        RetraiteDeBase = _calculateurCommun.CalculeLaRetraiteDeBase(revenu, TauxInchanges.CotisationsRetraiteBaseRevenusInferieursAuPass, TauxInchanges.CotisationsRetraiteBaseRevenusSuperieursAuPass, 5243m);
        RetraiteComplementaire = _calculateurCommun.CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(revenu, TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants, TauxInchanges.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants, _constantes.PlafondsRetraiteComplementaireArtisansCommercants);
        InvaliditeDeces = _calculateurCommun.CalculeLaCotisationInvaliditeDeces(revenu);
        AllocationsFamiliales = _calculateurCommun.CalculeLesAllocationsFamiliales(revenu);
        FormationProfessionnelle = _calculateurCommun.CalculeLaFormationProfessionnelle();

        var totalCotisationsObligatoires = MaladieHorsIndemnitesJournalieres.Valeur + MaladieIndemnitesJournalieres.Valeur + RetraiteDeBase.Valeur + RetraiteComplementaire.Valeur + InvaliditeDeces.Valeur + AllocationsFamiliales.Valeur;
        (CSGNonDeductible, CSGDeductible, CRDSNonDeductible) = CalculateurCommun.CalculeCSGEtCRDSAvant2025(revenu, totalCotisationsObligatoires);
    }

    private void CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette)
    {
        var tauxDeCotisationsIndemnitesMaladie = _constantes.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass;
        if (assiette < PASS.Valeur40Pct)
        {
            var cotisations = tauxDeCotisationsIndemnitesMaladie * PASS.Valeur40Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS), donc le taux fixe de {tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plancher de 40% du PASS, soit {cotisations:C0} de cotisations.", tauxDeCotisationsIndemnitesMaladie);
            return;
        }

        if (assiette < PASS.Valeur500Pct)
        {
            var cotisations = tauxDeCotisationsIndemnitesMaladie * assiette;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à cette assiette, soit {cotisations:C0} de cotisations.", tauxDeCotisationsIndemnitesMaladie);
        }
        else
        {
            var cotisations = tauxDeCotisationsIndemnitesMaladie * PASS.Valeur500Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plafond de 500% du PASS, soit {cotisations:C0} de cotisations.", tauxDeCotisationsIndemnitesMaladie);
        }
    }

    public ResultatAvecExplicationEtTaux MaladieHorsIndemnitesJournalieres { get; private set; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication MaladieIndemnitesJournalieres { get; private set; } = new ResultatVideSansExplication();
    public ResultatAvecTauxMultiplesEtExplication RetraiteDeBase { get; private set; } = new ResultatVideAvecTauxMultiplesEtSansExplication();
    public ResultatAvecTauxMultiplesEtExplication RetraiteComplementaire { get; private set; } = new ResultatVideAvecTauxMultiplesEtSansExplication();
    public ResultatAvecTauxUniqueEtExplication InvaliditeDeces { get; private set; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication AllocationsFamiliales { get; private set; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication CSGNonDeductible { get; private set; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication CSGDeductible { get; private set; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication CRDSNonDeductible { get; private set; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication FormationProfessionnelle { get; private set; } = new ResultatVideSansExplication();
    public decimal AssietteDeCalculDesCotisations { get; private set; }
}