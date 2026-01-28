using System.Globalization;

namespace Cotisations;

public class Calculateur2026 : ICalculateurPostReforme2024
{
    private const decimal TauxPlancherDeLAbattement = 0.0176m;
    private const decimal TauxPlafondDeLAbattement = 1.30m;
    private const decimal TauxAbattement = 0.26m;

    private readonly PlafondAnnuelSecuriteSociale PASS;
    private readonly CalculateurCommun _calculateurCommun;

    public Calculateur2026()
    {
        PASS = new PlafondAnnuelSecuriteSociale(2026);
        _calculateurCommun = new CalculateurCommun(PASS);
    }

    public void CalculeLesCotisations(decimal revenuBrut)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

        CalculeAbattementEtAssiette(revenuBrut);

        MaladieHorsIndemnitesJournalieres = CalculeLesCotisationsMaladieHorsIndemnites();
        MaladieIndemnitesJournalieres = CalculeLesCotisationsPourIndemnitesMaladie();
        RetraiteDeBase = _calculateurCommun.CalculeLaRetraiteDeBase(AssietteDeCalculDesCotisations, Constantes2026.TauxCotisationsRetraiteBaseRevenusInferieursAuPass, Constantes2026.TauxCotisationsRetraiteBaseRevenusSuperieursAuPass, Constantes2026.RevenusPlancherRetraiteDeBase);
        RetraiteComplementaire = _calculateurCommun.CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(AssietteDeCalculDesCotisations, Constantes2026.TauxRetraiteComplementairePremiereTrancheArtisansCommercants, Constantes2026.TauxRetraiteComplementaireDeuxiemeTrancheArtisansCommercants, PASS.Valeur);
        InvaliditeDeces = _calculateurCommun.CalculeLaCotisationInvaliditeDeces(AssietteDeCalculDesCotisations);
        AllocationsFamiliales = _calculateurCommun.CalculeLesAllocationsFamiliales(AssietteDeCalculDesCotisations);
        FormationProfessionnelle = _calculateurCommun.CalculeLaFormationProfessionnelle();

        CalculeCSGEtCRDS();
    }

    private void CalculeAbattementEtAssiette(decimal revenuBrut)
    {
        var abattementPlancher = TauxPlancherDeLAbattement * PASS.Valeur;
        var abattementPlafond = TauxPlafondDeLAbattement * PASS.Valeur;
        Abattement = Math.Min(Math.Max(revenuBrut * TauxAbattement, abattementPlancher), abattementPlafond);
        AssietteDeCalculDesCotisations = revenuBrut - Abattement;
        AssietteCsgCrds = AssietteDeCalculDesCotisations;
    }

    private ResultatAvecExplicationEtTaux CalculeLesCotisationsMaladieHorsIndemnites()
    {
        if (AssietteDeCalculDesCotisations > PASS.Valeur * 0.2m && AssietteDeCalculDesCotisations <= PASS.Valeur40Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(AssietteDeCalculDesCotisations, PASS.Valeur * 0.2m, PASS.Valeur40Pct, 0m, Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass);
            var cotisations = AssietteDeCalculDesCotisations * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est comprise entre {PASS.Valeur * 0.2m:C0} (20% du PASS) et {PASS.Valeur40Pct:C0} (40% du PASS), donc un taux progressif entre 0% et {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {cotisations:C0} de cotisations.", tauxApplicable);
        }

        if (AssietteDeCalculDesCotisations > PASS.Valeur40Pct && AssietteDeCalculDesCotisations <= PASS.Valeur60Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(AssietteDeCalculDesCotisations, PASS.Valeur40Pct, PASS.Valeur60Pct, Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass, Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass);
            var valeur = AssietteDeCalculDesCotisations * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur60Pct:C0} (60% du PASS), donc un taux progressif entre {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass * 100:F1}% et {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass * 100:N0}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (AssietteDeCalculDesCotisations > PASS.Valeur60Pct && AssietteDeCalculDesCotisations <= PASS.Valeur110Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(AssietteDeCalculDesCotisations, PASS.Valeur60Pct, PASS.Valeur110Pct, Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass, Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass);
            var valeur = AssietteDeCalculDesCotisations * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est comprise entre {PASS.Valeur60Pct:C0} (60% du PASS) et {PASS.Valeur110Pct:C0} (110% du PASS), donc un taux progressif entre {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass * 100:N0}% et {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (AssietteDeCalculDesCotisations > PASS.Valeur110Pct && AssietteDeCalculDesCotisations <= PASS.Valeur * 2)
        {
            var tauxApplicable = CalculeLeTauxProgressif(AssietteDeCalculDesCotisations, PASS.Valeur110Pct, PASS.Valeur * 2, Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass, Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass);
            var valeur = AssietteDeCalculDesCotisations * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur * 2:C0} (2 PASS), donc un taux progressif entre {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass * 100:F1}% et {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (AssietteDeCalculDesCotisations > PASS.Valeur * 2 && AssietteDeCalculDesCotisations <= PASS.Valeur * 3)
        {
            var tauxApplicable = CalculeLeTauxProgressif(AssietteDeCalculDesCotisations, PASS.Valeur * 2, PASS.Valeur * 3, Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass, Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass);
            var valeur = AssietteDeCalculDesCotisations * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est comprise entre {PASS.Valeur * 2:C0} (2 PASS) et {PASS.Valeur * 3:C0} (3 PASS), donc un taux progressif entre {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass * 100:F1}% et {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (AssietteDeCalculDesCotisations > PASS.Valeur * 3)
        {
            var partTauxMax = PASS.Valeur * 3;
            var partTauxReduit = AssietteDeCalculDesCotisations - partTauxMax;
            var valeur = partTauxMax * Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass + partTauxReduit * Constantes2026.TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass;
            return new ResultatAvecExplicationEtTaux(valeur, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est supérieure à {PASS.Valeur * 3:C0} (3 PASS), donc un taux de {Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass * 100:F1}% est appliqué à la part des revenus dans la limite de 3 PASS et le taux fixe de {Constantes2026.TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.", Constantes2026.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass, Constantes2026.TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass);
        }

        return new ResultatAvecTauxUniqueEtExplication(0m, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est inférieure à {PASS.Valeur40Pct:C0} (20% du PASS). Il n'y a donc pas de cotisation maladie à payer.", 0m);
    }

    private ResultatAvecTauxUniqueEtExplication CalculeLesCotisationsPourIndemnitesMaladie()
    {
        var tauxDeCotisationsIndemnitesMaladie = Constantes2026.TauxCotisationsIndemnitesMaladie;

        if (AssietteDeCalculDesCotisations < PASS.Valeur40Pct)
        {
            var cotisations = tauxDeCotisationsIndemnitesMaladie * PASS.Valeur40Pct;
            return new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS), donc le taux fixe de {tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plancher de 40% du PASS, soit {cotisations:C0} de cotisations.", tauxDeCotisationsIndemnitesMaladie);
        }

        if (AssietteDeCalculDesCotisations < PASS.Valeur500Pct)
        {
            var cotisations = tauxDeCotisationsIndemnitesMaladie * AssietteDeCalculDesCotisations;
            return new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à cette assiette, soit {cotisations:C0} de cotisations.", tauxDeCotisationsIndemnitesMaladie);
        }
        else
        {
            var cotisations = tauxDeCotisationsIndemnitesMaladie * PASS.Valeur500Pct;
            return new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {AssietteDeCalculDesCotisations:C0} est supérieure à {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plafond de 500% du PASS, soit {cotisations:C0} de cotisations.", tauxDeCotisationsIndemnitesMaladie);
        }
    }

    private void CalculeCSGEtCRDS()
    {
        var valeurCsgNonDeductible = AssietteCsgCrds * TauxInchanges.CSGNonDeductible;
        CSGNonDeductible = new ResultatAvecTauxUniqueEtExplication(valeurCsgNonDeductible, $"L'assiette de calcul de la CSG est égale au revenu moins un abattement de 26% soit {AssietteCsgCrds:C0}. Le taux fixe de {TauxInchanges.CSGNonDeductible * 100:F1}% est appliqué à cette assiette, ce qui donne une valeur de {valeurCsgNonDeductible:C0} pour la CSG non déductible.", TauxInchanges.CSGNonDeductible);

        var valeurCsgDeductible = AssietteCsgCrds * TauxInchanges.CSGDeductible;
        CSGDeductible = new ResultatAvecTauxUniqueEtExplication(valeurCsgDeductible, $"L'assiette de calcul de la CSG est égale au revenu moins un abattement de 26% soit {AssietteCsgCrds:C0}. Le taux fixe de {TauxInchanges.CSGDeductible * 100:F1}% est appliqué à cette assiette, ce qui donne une valeur de une valeur de {valeurCsgDeductible:C0} pour la CSG déductible.", TauxInchanges.CSGDeductible);

        var valeurCrds = AssietteCsgCrds * TauxInchanges.CRDSNonDeductible;
        CRDSNonDeductible = new ResultatAvecTauxUniqueEtExplication(valeurCrds, $"L'assiette de calcul de la CRDS est égale au revenu moins un abattement de 26% soit {AssietteCsgCrds:C0}. Le taux fixe de {TauxInchanges.CRDSNonDeductible * 100:F1}% est appliqué à cette assiette, ce qui donne une valeur de {valeurCrds:C0} pour la CRDS.", TauxInchanges.CRDSNonDeductible);
    }

    // TODO: bouger dans un helper ?
    private static decimal CalculeLeTauxProgressif(decimal assiette, decimal valeurPlancher, decimal valeurPlafond, decimal tauxPlancher, decimal tauxPlafond)
    {
        var diffDeTaux = tauxPlafond - tauxPlancher;
        var ratio = (assiette - valeurPlancher) / (valeurPlafond - valeurPlancher);
        var tauxApplicable = diffDeTaux * ratio + tauxPlancher;

        return tauxApplicable;
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
    public decimal AssietteCsgCrds { get; private set; }
    public decimal AssietteDeCalculDesCotisations { get; private set; }
    public decimal Abattement { get; private set; }
}