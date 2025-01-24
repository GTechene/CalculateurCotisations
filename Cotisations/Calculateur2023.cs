using System.Globalization;

namespace Cotisations;

public class Calculateur2023 : ICalculateur
{
    private readonly PlafondAnnuelSecuriteSociale PASS;
    private readonly Constantes2023 _constantes;
    private readonly CalculateurCommun _calculateurCommun;

    public Calculateur2023()
    {
        PASS = new PlafondAnnuelSecuriteSociale(2023);
        _constantes = new Constantes2023();
        _calculateurCommun = new CalculateurCommun(PASS);
    }

    public void CalculeLesCotisations(decimal revenu)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

        AssietteDeCalculDesCotisations = revenu;
        MaladieHorsIndemnitesJournalieres = _calculateurCommun.CalculeLesCotisationsMaladieHorsIndemnitesJournalieresAvant2025(revenu, _constantes.CotisationsMaladiePourRevenusSupA60PctPass, _constantes.CotisationsMaladiePourRevenusSupA5Pass);
        CalculeLesCotisationsPourIndemnitesMaladie(revenu);
        RetraiteDeBase = _calculateurCommun.CalculeLaRetraiteDeBase(revenu, TauxInchanges.CotisationsRetraiteBaseRevenusInferieursAuPass, TauxInchanges.CotisationsRetraiteBaseRevenusSuperieursAuPass);
        RetraiteComplementaire = _calculateurCommun.CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(revenu, TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants, TauxInchanges.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants, _constantes.PlafondsRetraiteComplementaireArtisansCommercants);
        InvaliditeDeces = _calculateurCommun.CalculeLaCotisationInvaliditeDeces(revenu);
        AllocationsFamiliales = _calculateurCommun.CalculeLesAllocationsFamiliales(revenu);
        FormationProfessionnelle = _calculateurCommun.CalculeLaFormationProfessionnelle();

        var totalCotisationsObligatoires = MaladieHorsIndemnitesJournalieres.Valeur + MaladieIndemnitesJournalieres.Valeur + RetraiteDeBase.Valeur + RetraiteComplementaire.Valeur + InvaliditeDeces.Valeur + AllocationsFamiliales.Valeur;
        (CSGNonDeductible, CSGDeductible, CRDSNonDeductible) = CalculateurCommun.CalculeCSGEtCRDSAvant2025(revenu, totalCotisationsObligatoires);
    }

    private void CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette)
    {
        if (assiette < PASS.Valeur40Pct)
        {
            var cotisations = _constantes.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass * PASS.Valeur40Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS), donc le taux fixe de {_constantes.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass * 100:F1}% est appliqué à ce plancher de 40% du PASS, soit {cotisations:C0} de cotisations.", _constantes.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass);
            return;
        }

        if (assiette > PASS.Valeur40Pct && assiette <= PASS.Valeur110Pct)
        {
            var difference = assiette - PASS.Valeur40Pct;
            var differenceEntrePlafondEtPlancher = PASS.Valeur110Pct - PASS.Valeur40Pct;
            var tauxApplicable = (difference / differenceEntrePlafondEtPlancher) * (_constantes.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass - _constantes.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass) + _constantes.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass;
            var cotisations = tauxApplicable * assiette;

            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur110Pct} (60% du PASS), donc le taux progressif de {tauxApplicable * 100:F2}% est appliqué, soit {cotisations:C0} de cotisations.", tauxApplicable);
            return;
        }

        if (assiette < PASS.Valeur500Pct)
        {
            var cotisations = _constantes.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass * assiette;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {_constantes.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass * 100:F2}% est appliqué à cette assiette, soit {cotisations:C0} de cotisations.", _constantes.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass);
        }
        else
        {
            var cotisations = _constantes.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass * PASS.Valeur500Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur110Pct:C0} (110% du PASS), donc le taux fixe de {_constantes.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass * 100:F2}% est appliqué à ce plafond de 500% du PASS, soit {cotisations:C0} de cotisations.", _constantes.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass);
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