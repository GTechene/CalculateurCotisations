namespace Cotisations;

public class Calculateur2023() : CalculateurDeBase(new Constantes2023(), new PlafondAnnuelSecuriteSociale(2023))
{
    protected override void CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette)
    {
        if (assiette < PASS.Valeur40Pct)
        {
            var cotisations = ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass * PASS.Valeur40Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS), donc le taux fixe de {ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass * 100:F1}% est appliqué à ce plancher de 40% du PASS, soit {cotisations:C0} de cotisations.", ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass);
            return;
        }

        if (assiette > PASS.Valeur40Pct && assiette <= PASS.Valeur110Pct)
        {
            var difference = assiette - PASS.Valeur40Pct;
            var differenceEntrePlafondEtPlancher = PASS.Valeur110Pct - PASS.Valeur40Pct;
            var tauxApplicable = (difference / differenceEntrePlafondEtPlancher) * (ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass - ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass) + ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass;
            var cotisations = tauxApplicable * assiette;

            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur110Pct} (60% du PASS), donc le taux progressif de {tauxApplicable * 100:F2}% est appliqué, soit {cotisations:C0} de cotisations.", tauxApplicable);
            return;
        }

        if (assiette < PASS.Valeur500Pct)
        {
            var cotisations = ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass * assiette;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass * 100:F2}% est appliqué à cette assiette, soit {cotisations:C0} de cotisations.", ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass);
        }
        else
        {
            var cotisations = ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass * PASS.Valeur500Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur110Pct:C0} (110% du PASS), donc le taux fixe de {ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass * 100:F2}% est appliqué à ce plafond de 500% du PASS, soit {cotisations:C0} de cotisations.", ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusSuperieursA110PctDuPass);
        }
    }

    protected override void CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(decimal assiette)
    {
        if (assiette <= ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants)
        {
            var valeur = assiette * TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants;
            RetraiteComplementaire = new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants * 100:N0}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.", TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants, 0m);
        }
        else
        {
            var cotisationPremiereTranche = TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants * ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants;
            var cotisationDeuxiemeTranche = TauxInchanges.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * (Math.Min(assiette, PASS.Valeur400Pct) - ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants);

            var cotisations = cotisationPremiereTranche + cotisationDeuxiemeTranche;
            RetraiteComplementaire = new ResultatAvecTauxMultiplesEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants * 100:N0}% est appliqué à la part des revenus inférieure à cette valeur et le taux fixe de {TauxInchanges.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * 100:N0}% est appliqué à la part des revenus qui y est supérieure, soit {cotisations:C0} de cotisations.", TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants, TauxInchanges.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants);
        }
    }
}