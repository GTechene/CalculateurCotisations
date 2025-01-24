namespace Cotisations;

public class Calculateur2025 : ICalculateur
{
    private const decimal TauxPlancherDeLAbattement = 0.0176m;
    private const decimal TauxPlafondDeLAbattement = 1.30m;
    private const decimal TauxAbattement = 0.26m;

    private readonly PlafondAnnuelSecuriteSociale PASS;
    private readonly Constantes2025 _constantes;

    public Calculateur2025()
    {
        PASS = new PlafondAnnuelSecuriteSociale(2025);
        _constantes = new Constantes2025();
    }

    public void CalculeLesCotisations(decimal revenu)
    {
        var abattementPlancher = TauxPlancherDeLAbattement * PASS.Valeur;
        var abattementPlafond = TauxPlafondDeLAbattement * PASS.Valeur;
        var abattement = Math.Min(Math.Max(revenu * TauxAbattement, abattementPlancher),  abattementPlafond);

        AssietteCsgCrds = revenu - abattement;

        MaladieHorsIndemnitesJournalieres = CalculeLesCotisationsMaladieHorsIndemnites(revenu);
        MaladieIndemnitesJournalieres = CalculeLesCotisationsPourIndemnitesMaladie(revenu);
        RetraiteDeBase = CalculeLaRetraiteDeBase(revenu);
        RetraiteComplementaire = CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(revenu);
    }

    private ResultatAvecExplicationEtTaux CalculeLesCotisationsMaladieHorsIndemnites(decimal revenu)
    {
        if (revenu > PASS.Valeur * 0.2m && revenu <= PASS.Valeur40Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur * 0.2m, PASS.Valeur40Pct, 0m, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass);
            var cotisations = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(cotisations, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur * 0.2m:C0} (20% du PASS) et {PASS.Valeur40Pct:C0} (40% du PASS), donc un taux progressif entre 0% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {cotisations:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur40Pct && revenu <= PASS.Valeur60Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur40Pct, PASS.Valeur60Pct, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass);
            var valeur = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur60Pct:C0} (60% du PASS), donc un taux progressif entre {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass * 100:F1}% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass * 100:N0}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur60Pct && revenu <= PASS.Valeur110Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur60Pct, PASS.Valeur110Pct, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass);
            var valeur = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur60Pct:C0} (60% du PASS) et {PASS.Valeur110Pct:C0} (110% du PASS), donc un taux progressif entre {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass * 100:N0}% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur110Pct && revenu <= PASS.Valeur * 2)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur110Pct, PASS.Valeur * 2, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass);
            var valeur = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur * 2:C0} (2 PASS), donc un taux progressif entre {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass * 100:F1}% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur * 2 && revenu <= PASS.Valeur * 3)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur * 2, PASS.Valeur * 3, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass);
            var valeur = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur * 2:C0} (2 PASS) et {PASS.Valeur * 3:C0} (3 PASS), donc un taux progressif entre {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass * 100:F1}% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur * 3)
        {
            var partTauxMax = PASS.Valeur * 3;
            var partTauxReduit = revenu - partTauxMax;
            var valeur = partTauxMax * _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass + partTauxReduit * _constantes.TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass;
            return new ResultatAvecExplicationEtTaux(valeur, $"Le montant de {revenu:C0} est supérieur à {PASS.Valeur * 3:C0} (3 PASS), donc un taux de {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass * 100:F1}% est appliqué à la part des revenus dans la limite de 3 PASS et le taux fixe de {_constantes.TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.", _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass, _constantes.TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass);
        }

        return new ResultatAvecTauxUniqueEtExplication(0m, $"Le montant de {revenu:C0} est inférieur à {PASS.Valeur40Pct:C0} (20% du PASS). Il n'y a donc pas de cotisation maladie à payer.", 0m);
    }

    private ResultatAvecTauxUniqueEtExplication CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette)
    {
        var tauxDeCotisationsIndemnitesMaladie = _constantes.TauxCotisationsIndemnitesMaladie;

        if (assiette < PASS.Valeur40Pct)
        {
            var cotisations = tauxDeCotisationsIndemnitesMaladie * PASS.Valeur40Pct;
            return new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS), donc le taux fixe de {tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plancher de 40% du PASS, soit {cotisations:C0} de cotisations.", tauxDeCotisationsIndemnitesMaladie);
        }

        if (assiette < PASS.Valeur500Pct)
        {
            var cotisations = tauxDeCotisationsIndemnitesMaladie * assiette;
            return new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à cette assiette, soit {cotisations:C0} de cotisations.", tauxDeCotisationsIndemnitesMaladie);
        }
        else
        {
            var cotisations = tauxDeCotisationsIndemnitesMaladie * PASS.Valeur500Pct;
            return new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plafond de 500% du PASS, soit {cotisations:C0} de cotisations.", tauxDeCotisationsIndemnitesMaladie);
        }
    }

    private ResultatAvecTauxMultiplesEtExplication CalculeLaRetraiteDeBase(decimal assiette)
    {
        var tauxPourRevenusInferieursAuPass = 0.1787m;
        if (assiette <= PASS.Valeur)
        {
            var valeur = assiette * tauxPourRevenusInferieursAuPass;
            return new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur:C0} (PASS), donc le taux fixe de {tauxPourRevenusInferieursAuPass * 100:F2}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.", tauxPourRevenusInferieursAuPass, 0m);
        }
        else
        {
            var tauxPourRevenusSuperieursAuPass = 0.072m;
            var depassementDuPass = assiette - PASS.Valeur;

            var cotisationsSurPass = PASS.Valeur * tauxPourRevenusInferieursAuPass;
            var cotisationsSurDepassement = depassementDuPass * tauxPourRevenusSuperieursAuPass;

            var valeur = cotisationsSurPass + cotisationsSurDepassement;
            return new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur:C0} (PASS), donc le taux fixe de {tauxPourRevenusInferieursAuPass * 100:F2}% est appliqué à la part des revenus inférieure au PASS et le taux de {tauxPourRevenusSuperieursAuPass * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.", tauxPourRevenusInferieursAuPass, tauxPourRevenusSuperieursAuPass);
        }
    }

    private ResultatAvecTauxMultiplesEtExplication CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(decimal assiette)
    {
        var plafondsRetraiteComplementaireArtisansCommercants = _constantes.PlafondsRetraiteComplementaireArtisansCommercants;
        var tauxPremiereTranche = 0.081m;
        if (assiette <= plafondsRetraiteComplementaireArtisansCommercants)
        {
            var valeur = assiette * tauxPremiereTranche;
            return new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {plafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {tauxPremiereTranche * 100:N0}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.", tauxPremiereTranche, 0m);
        }

        var cotisationPremiereTranche = tauxPremiereTranche * plafondsRetraiteComplementaireArtisansCommercants;
        var tauxDeuxiemeTranche = 0.091m;
        var cotisationDeuxiemeTranche = tauxDeuxiemeTranche * (Math.Min(assiette, PASS.Valeur400Pct) - plafondsRetraiteComplementaireArtisansCommercants);

        var cotisations = cotisationPremiereTranche + cotisationDeuxiemeTranche;
        return new ResultatAvecTauxMultiplesEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {plafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {tauxPremiereTranche * 100:N0}% est appliqué à la part des revenus inférieure à cette valeur et le taux fixe de {tauxDeuxiemeTranche * 100:N0}% est appliqué à la part des revenus qui y est supérieure, soit {cotisations:C0} de cotisations.", tauxPremiereTranche, tauxDeuxiemeTranche);
    }

    // TODO: bouger dans un helper ? Genre une méthode d'extension sur le decimal en 1er param
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
    public ResultatAvecTauxUniqueEtExplication InvaliditeDeces { get; }
    public ResultatAvecTauxUniqueEtExplication AllocationsFamiliales { get; }
    public ResultatAvecTauxUniqueEtExplication CSGNonDeductible { get; }
    public ResultatAvecTauxUniqueEtExplication CSGDeductible { get; }
    public ResultatAvecTauxUniqueEtExplication CRDSNonDeductible { get; }
    public ResultatAvecTauxUniqueEtExplication FormationProfessionnelle { get; }
    public decimal AssietteCsgCrds { get; private set; }
}