﻿using System.Globalization;

namespace Cotisations;

public class Calculateur2025 : ICalculateur
{
    private const decimal TauxPlancherDeLAbattement = 0.0176m;
    private const decimal TauxPlafondDeLAbattement = 1.30m;
    private const decimal TauxAbattement = 0.26m;

    private readonly PlafondAnnuelSecuriteSociale PASS;
    private readonly Constantes2025 _constantes;
    private readonly CalculateurCommun _calculateurCommun;

    public Calculateur2025()
    {
        PASS = new PlafondAnnuelSecuriteSociale(2025);
        _constantes = new Constantes2025();
        _calculateurCommun = new CalculateurCommun(PASS);
    }

    public void CalculeLesCotisations(decimal revenu)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

        AssietteDeCalculDesCotisations = revenu;
        MaladieHorsIndemnitesJournalieres = CalculeLesCotisationsMaladieHorsIndemnites(revenu);
        MaladieIndemnitesJournalieres = CalculeLesCotisationsPourIndemnitesMaladie(revenu);
        RetraiteDeBase = _calculateurCommun.CalculeLaRetraiteDeBase(revenu, _constantes.TauxCotisationsRetraiteBaseRevenusInferieursAuPass, _constantes.TauxCotisationsRetraiteBaseRevenusSuperieursAuPass, 5346m);
        RetraiteComplementaire = _calculateurCommun.CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(revenu, _constantes.TauxRetraiteComplementairePremiereTrancheArtisansCommercants, _constantes.TauxRetraiteComplementaireDeuxiemeTrancheArtisansCommercants, _constantes.PlafondsRetraiteComplementaireArtisansCommercants);
        InvaliditeDeces = _calculateurCommun.CalculeLaCotisationInvaliditeDeces(revenu);
        AllocationsFamiliales = _calculateurCommun.CalculeLesAllocationsFamiliales(revenu);
        FormationProfessionnelle = _calculateurCommun.CalculeLaFormationProfessionnelle();
        
        CalculeCSGEtCRDS(revenu);
    }

    private ResultatAvecExplicationEtTaux CalculeLesCotisationsMaladieHorsIndemnites(decimal revenu)
    {
        if (revenu > PASS.Valeur * 0.2m && revenu <= PASS.Valeur40Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur * 0.2m, PASS.Valeur40Pct, 0m, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass);
            var cotisations = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {revenu:C0} est compris entre {PASS.Valeur * 0.2m:C0} (20% du PASS) et {PASS.Valeur40Pct:C0} (40% du PASS), donc un taux progressif entre 0% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {cotisations:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur40Pct && revenu <= PASS.Valeur60Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur40Pct, PASS.Valeur60Pct, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass);
            var valeur = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {revenu:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur60Pct:C0} (60% du PASS), donc un taux progressif entre {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre20PctEt40PctDuPass * 100:F1}% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass * 100:N0}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur60Pct && revenu <= PASS.Valeur110Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur60Pct, PASS.Valeur110Pct, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass);
            var valeur = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {revenu:C0} est comprise entre {PASS.Valeur60Pct:C0} (60% du PASS) et {PASS.Valeur110Pct:C0} (110% du PASS), donc un taux progressif entre {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre40PctEt60PctDuPass * 100:N0}% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur110Pct && revenu <= PASS.Valeur * 2)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur110Pct, PASS.Valeur * 2, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass);
            var valeur = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {revenu:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur * 2:C0} (2 PASS), donc un taux progressif entre {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre60PctEt110PctDuPass * 100:F1}% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur * 2 && revenu <= PASS.Valeur * 3)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur * 2, PASS.Valeur * 3, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass, _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass);
            var valeur = revenu * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {revenu:C0} est comprise entre {PASS.Valeur * 2:C0} (2 PASS) et {PASS.Valeur * 3:C0} (3 PASS), donc un taux progressif entre {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre110PctEt200PctDuPass * 100:F1}% et {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (revenu > PASS.Valeur * 3)
        {
            var partTauxMax = PASS.Valeur * 3;
            var partTauxReduit = revenu - partTauxMax;
            var valeur = partTauxMax * _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass + partTauxReduit * _constantes.TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass;
            return new ResultatAvecExplicationEtTaux(valeur, $"L'assiette de {revenu:C0} est supérieure à {PASS.Valeur * 3:C0} (3 PASS), donc un taux de {_constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass * 100:F1}% est appliqué à la part des revenus dans la limite de 3 PASS et le taux fixe de {_constantes.TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.", _constantes.TauxPlafondCotisationsMaladiePourRevenusEntre200PctEt300PctDuPass, _constantes.TauxPartReduiteCotisationsMaladiePourRevenusSupA300PctDuPass);
        }

        return new ResultatAvecTauxUniqueEtExplication(0m, $"L'assiette de {revenu:C0} est inférieure à {PASS.Valeur40Pct:C0} (20% du PASS). Il n'y a donc pas de cotisation maladie à payer.", 0m);
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

    private void CalculeCSGEtCRDS(decimal revenu)
    {
        var abattementPlancher = TauxPlancherDeLAbattement * PASS.Valeur;
        var abattementPlafond = TauxPlafondDeLAbattement * PASS.Valeur;
        var abattement = Math.Min(Math.Max(revenu * TauxAbattement, abattementPlancher), abattementPlafond);

        AssietteCsgCrds = revenu - abattement;

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
}