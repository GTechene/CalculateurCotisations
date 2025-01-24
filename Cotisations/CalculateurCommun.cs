namespace Cotisations;

/// <summary>
/// Le but de cette classe est d'exposer une factory method qui appelle les méthodes virtuelles que les classes dérivées devront implémenter. Pour l'instant, seules certaines cotisations se calculent différemment d'année en année, donc seules ces méthodes sont exposées comme virtuelles pures, les autres sont implémentées ici.
/// </summary>
public class CalculateurCommun(PlafondAnnuelSecuriteSociale pass)
{
    public ResultatAvecTauxUniqueEtExplication CalculeLesCotisationsMaladieHorsIndemnitesJournalieresAvant2025(decimal assiette, decimal cotisationsMaladiePourRevenusSupA60PctPass, decimal cotisationsMaladiePourRevenusSupA5Pass)
    {
        if (assiette <= pass.Valeur40Pct)
        {
            return new ResultatAvecTauxUniqueEtExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {pass.Valeur40Pct:C0} (40% du PASS). Il n'y a donc pas de cotisation maladie à payer.", 0m);
        }

        if (assiette > pass.Valeur40Pct && assiette <= pass.Valeur60Pct)
        {
            var difference = assiette - pass.Valeur40Pct;
            var differenceEntrePlafondEtPlancher = pass.Valeur60Pct - pass.Valeur40Pct;
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * TauxInchanges.CotisationsMaladiePourRevenusInferieursA60PctDuPass;
            var valeur = tauxApplicable * assiette;

            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {pass.Valeur40Pct:C0} (40% du PASS) et {pass.Valeur60Pct:C0} (60% du PASS), donc un taux progressif entre 0% et {TauxInchanges.CotisationsMaladiePourRevenusInferieursA60PctDuPass * 100:N0}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (assiette > pass.Valeur60Pct && assiette <= pass.Valeur110Pct)
        {
            var difference = assiette - pass.Valeur60Pct;
            var differenceEntrePlafondEtPlancher = pass.Valeur110Pct - pass.Valeur60Pct;
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * (cotisationsMaladiePourRevenusSupA60PctPass - TauxInchanges.CotisationsMaladiePourRevenusInferieursA60PctDuPass) + TauxInchanges.CotisationsMaladiePourRevenusInferieursA60PctDuPass;
            var valeur = tauxApplicable * assiette;

            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {pass.Valeur60Pct:C0} (60% du PASS) et {pass.Valeur110Pct:C0} (110% du PASS), donc un taux progressif entre 4% et {cotisationsMaladiePourRevenusSupA60PctPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (assiette > pass.Valeur110Pct && assiette <= pass.Valeur500Pct)
        {
            var valeur = cotisationsMaladiePourRevenusSupA60PctPass * assiette;

            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {pass.Valeur110Pct:C0} (110% du PASS) et {pass.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {cotisationsMaladiePourRevenusSupA60PctPass * 100:F1}% est appliqué, soit {valeur:C0} de cotisations.", cotisationsMaladiePourRevenusSupA60PctPass);
        }

        var differenceEntreAssietteEt5Pass = assiette - pass.Valeur500Pct;
        var valeurMax = cotisationsMaladiePourRevenusSupA60PctPass * pass.Valeur500Pct + cotisationsMaladiePourRevenusSupA5Pass * differenceEntreAssietteEt5Pass;

        return new ResultatAvecTauxUniqueEtExplication(valeurMax, $"L'assiette de {assiette:C0} est supérieure à {pass.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {cotisationsMaladiePourRevenusSupA60PctPass * 100:F1}% est appliqué à la tranche de revenus inférieure à cette valeur et le taux fixe de {cotisationsMaladiePourRevenusSupA5Pass * 100:F1}% est appliqué au reste. Soit {valeurMax:C0} de cotisations.", cotisationsMaladiePourRevenusSupA5Pass);
    }

    public ResultatAvecTauxMultiplesEtExplication CalculeLaRetraiteDeBase(decimal assiette, decimal tauxPourRevenusInferieursAuPass, decimal tauxPourRevenusSuperieursAuPass)
    {
        if (assiette <= pass.Valeur)
        {
            var valeur = assiette * tauxPourRevenusInferieursAuPass;
            return new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {pass.Valeur:C0} (PASS), donc le taux fixe de {tauxPourRevenusInferieursAuPass * 100:F2}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.", tauxPourRevenusInferieursAuPass, 0m);
        }
        else
        {
            var depassementDuPass = assiette - pass.Valeur;

            var cotisationsSurPass = pass.Valeur * tauxPourRevenusInferieursAuPass;
            var cotisationsSurDepassement = depassementDuPass * tauxPourRevenusSuperieursAuPass;

            var valeur = cotisationsSurPass + cotisationsSurDepassement;
            return new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {pass.Valeur:C0} (PASS), donc le taux fixe de {tauxPourRevenusInferieursAuPass * 100:F2}% est appliqué à la part des revenus inférieure au PASS et le taux de {tauxPourRevenusSuperieursAuPass * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.", tauxPourRevenusInferieursAuPass, tauxPourRevenusSuperieursAuPass);
        }
    }

    public ResultatAvecTauxMultiplesEtExplication CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(decimal assiette, decimal tauxPremiereTranche, decimal tauxDeuxiemeTranche, decimal plafondsRetraiteComplementaireArtisansCommercants)
    {
        if (assiette <= plafondsRetraiteComplementaireArtisansCommercants)
        {
            var valeur = assiette * tauxPremiereTranche;
            return new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {plafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {tauxPremiereTranche * 100:N0}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.", tauxPremiereTranche, 0m);
        }

        var cotisationPremiereTranche = tauxPremiereTranche * plafondsRetraiteComplementaireArtisansCommercants;
        var cotisationDeuxiemeTranche = tauxDeuxiemeTranche * (Math.Min(assiette, pass.Valeur400Pct) - plafondsRetraiteComplementaireArtisansCommercants);

        var cotisations = cotisationPremiereTranche + cotisationDeuxiemeTranche;
        return new ResultatAvecTauxMultiplesEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {plafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {tauxPremiereTranche * 100:N0}% est appliqué à la part des revenus inférieure à cette valeur et le taux fixe de {tauxDeuxiemeTranche * 100:N0}% est appliqué à la part des revenus qui y est supérieure, soit {cotisations:C0} de cotisations.", tauxPremiereTranche, tauxDeuxiemeTranche);
    }

    public ResultatAvecTauxUniqueEtExplication CalculeLaCotisationInvaliditeDeces(decimal assiette)
    {
        var valeur = Math.Min(assiette, pass.Valeur) * TauxInchanges.InvaliditeDeces;
        if (assiette <= pass.Valeur)
        {
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure ou égale au PASS ({pass.Valeur:C0}). Le taux de {TauxInchanges.InvaliditeDeces * 100} % est donc directement appliqué à cette assiette, soit {valeur:C0} de cotisations.", TauxInchanges.InvaliditeDeces);
        }

        return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure au PASS ({pass.Valeur:C0}). Le taux de {TauxInchanges.InvaliditeDeces * 100:F1} % est donc appliqué au PASS, soit {valeur:C0} de cotisations.", TauxInchanges.InvaliditeDeces);
    }

    public ResultatAvecTauxUniqueEtExplication CalculeLesAllocationsFamiliales(decimal assiette)
    {
        if (assiette <= pass.Valeur110Pct)
        {
            return new ResultatAvecTauxUniqueEtExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {pass.Valeur110Pct:C0} (110% du PASS). Il n'y a donc pas de cotisation à payer pour les allocations familiales.", 0m);
        }

        if (assiette > pass.Valeur110Pct && assiette <= pass.Valeur140Pct)
        {
            var difference = assiette - pass.Valeur110Pct;
            var differenceEntrePlafondsEtPlancher = pass.Valeur140Pct - pass.Valeur110Pct;
            var tauxApplicable = difference / differenceEntrePlafondsEtPlancher * TauxInchanges.CotisationsAllocationsFamiliales;

            var valeur = assiette * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {pass.Valeur110Pct:C0} (110% du PASS) et {pass.Valeur140Pct:C0} (140% du PASS). Un taux progressif entre 0% et {TauxInchanges.CotisationsAllocationsFamiliales * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F2}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }
        else
        {
            var valeur = assiette * TauxInchanges.CotisationsAllocationsFamiliales;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {pass.Valeur140Pct:C0} (140% du PASS) donc un taux fixe de {TauxInchanges.CotisationsAllocationsFamiliales * 100:F1}% est appliqué, soit {valeur:C0} de cotisations.", TauxInchanges.CotisationsAllocationsFamiliales);
        }
    }

    public static (ResultatAvecTauxUniqueEtExplication csgNonDeductible, ResultatAvecTauxUniqueEtExplication csgDeductible, ResultatAvecTauxUniqueEtExplication crdsNonDeductible)
        CalculeCSGEtCRDSAvant2025(decimal assiette, decimal totalCotisationsObligatoires)
    {
        var revenusPrisEnCompte = assiette + totalCotisationsObligatoires;

        var valeurCsgNonDeductible = revenusPrisEnCompte * TauxInchanges.CSGNonDeductible;
        var csgNonDeductible = new ResultatAvecTauxUniqueEtExplication(valeurCsgNonDeductible, $"L'assiette de calcul de la CSG est égale à l'assiette retenue pour les cotisations obligatoires ({assiette:C0}) + les cotisations obligatoires elles-mêmes ({totalCotisationsObligatoires:C0}), soit un total de {revenusPrisEnCompte:C0}. Le taux fixe de {TauxInchanges.CSGNonDeductible * 100:F1}% est appliqué à cette assiette, ce qui donne une valeur de {valeurCsgNonDeductible:C0} pour la CSG non déductible.", TauxInchanges.CSGNonDeductible);

        var valeurCsgDeductible = revenusPrisEnCompte * TauxInchanges.CSGDeductible;
        var csgDeductible = new ResultatAvecTauxUniqueEtExplication(valeurCsgDeductible, $"L'assiette de calcul de la CSG est égale à l'assiette retenue pour les cotisations obligatoires ({assiette:C0}) + les cotisations obligatoires elles-mêmes ({totalCotisationsObligatoires:C0}), soit un total de {revenusPrisEnCompte:C0}. Le taux fixe de {TauxInchanges.CSGDeductible * 100:F1}% est appliqué à cette assiette, ce qui donne une valeur de une valeur de {valeurCsgDeductible:C0} pour la CSG déductible.", TauxInchanges.CSGDeductible);

        var valeurCrds = revenusPrisEnCompte * TauxInchanges.CRDSNonDeductible;
        var crdsNonDeductible = new ResultatAvecTauxUniqueEtExplication(valeurCrds, $"L'assiette de calcul de la CRDS est égale à l'assiette retenue pour les cotisations obligatoires ({assiette:C0}) + les cotisations obligatoires elles-mêmes ({totalCotisationsObligatoires:C0}), soit un total de {revenusPrisEnCompte:C0}. Le taux fixe de {TauxInchanges.CRDSNonDeductible * 100:F1}% est appliqué à cette assiette, ce qui donne une valeur de {valeurCrds:C0} pour la CRDS.", TauxInchanges.CRDSNonDeductible);

        return (csgNonDeductible, csgDeductible, crdsNonDeductible);
    }

    public ResultatAvecTauxUniqueEtExplication CalculeLaFormationProfessionnelle()
    {
        var valeur = pass.Valeur * TauxInchanges.CotisationsFormationProfessionnelle;
        return new ResultatAvecTauxUniqueEtExplication(valeur, $"Un taux fixe de {TauxInchanges.CotisationsFormationProfessionnelle * 100:F2}% est appliqué sur la valeur d'un PASS complet qui vaut {pass.Valeur:C0}, soit {valeur:C0} de cotisations.", TauxInchanges.CotisationsFormationProfessionnelle);
    }
}