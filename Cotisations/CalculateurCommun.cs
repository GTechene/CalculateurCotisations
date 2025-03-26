namespace Cotisations;

/// <summary>
/// Le but de cette classe est de centraliser les méthodes de calcul quand elles sont similaires.
/// </summary>
public class CalculateurCommun(PlafondAnnuelSecuriteSociale PASS)
{
    public ResultatAvecTauxUniqueEtExplication CalculeLesCotisationsMaladieHorsIndemnitesJournalieresAvant2025(decimal assiette, decimal tauxCotisationsMaladiePourRevenusInferieursA60PctDuPass, decimal tauxCotisationsMaladiePourRevenusSupA60PctPass)
    {
        if (assiette <= PASS.Valeur40Pct)
        {
            return new ResultatAvecTauxUniqueEtExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS). Il n'y a donc pas de cotisation maladie à payer.", 0m);
        }

        if (assiette > PASS.Valeur40Pct && assiette <= PASS.Valeur60Pct)
        {
            var difference = assiette - PASS.Valeur40Pct;
            var differenceEntrePlafondEtPlancher = PASS.Valeur60Pct - PASS.Valeur40Pct;
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * tauxCotisationsMaladiePourRevenusInferieursA60PctDuPass;
            var valeur = tauxApplicable * assiette;

            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur60Pct:C0} (60% du PASS), donc un taux progressif entre 0% et {tauxCotisationsMaladiePourRevenusInferieursA60PctDuPass * 100:N0}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (assiette > PASS.Valeur60Pct && assiette <= PASS.Valeur110Pct)
        {
            var difference = assiette - PASS.Valeur60Pct;
            var differenceEntrePlafondEtPlancher = PASS.Valeur110Pct - PASS.Valeur60Pct;
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * (tauxCotisationsMaladiePourRevenusSupA60PctPass - tauxCotisationsMaladiePourRevenusInferieursA60PctDuPass) + tauxCotisationsMaladiePourRevenusInferieursA60PctDuPass;
            var valeur = tauxApplicable * assiette;

            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur60Pct:C0} (60% du PASS) et {PASS.Valeur110Pct:C0} (110% du PASS), donc un taux progressif entre 4% et {tauxCotisationsMaladiePourRevenusSupA60PctPass * 100:F2}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }

        if (assiette > PASS.Valeur110Pct && assiette <= PASS.Valeur500Pct)
        {
            var valeur = tauxCotisationsMaladiePourRevenusSupA60PctPass * assiette;

            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {tauxCotisationsMaladiePourRevenusSupA60PctPass * 100:F2}% est appliqué, soit {valeur:C0} de cotisations.", tauxCotisationsMaladiePourRevenusSupA60PctPass);
        }

        var differenceEntreAssietteEt5Pass = assiette - PASS.Valeur500Pct;
        var valeurMax = tauxCotisationsMaladiePourRevenusSupA60PctPass * PASS.Valeur500Pct + TauxInchanges.CotisationsMaladiePourRevenusSupA5Pass * differenceEntreAssietteEt5Pass;

        return new ResultatAvecTauxUniqueEtExplication(valeurMax, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {tauxCotisationsMaladiePourRevenusSupA60PctPass * 100:F2}% est appliqué à la tranche de revenus inférieure à cette valeur et le taux fixe de {TauxInchanges.CotisationsMaladiePourRevenusSupA5Pass * 100:F2}% est appliqué au reste. Soit {valeurMax:C0} de cotisations.", TauxInchanges.CotisationsMaladiePourRevenusSupA5Pass);
    }

    public ResultatAvecTauxMultiplesEtExplication CalculeLaRetraiteDeBase(decimal assiette, decimal tauxPourRevenusInferieursAuPass, decimal tauxPourRevenusSuperieursAuPass, decimal plancher)
    {
        if (assiette <= PASS.Valeur)
        {
            if (assiette < plancher)
            {
                var valeurPlancher = plancher * tauxPourRevenusInferieursAuPass;
                return new ResultatAvecTauxMultiplesEtExplication(valeurPlancher, $"L'assiette de {assiette:C0} est inférieure au plancher ({TauxInchanges.PlancherRetraiteDeBase * 100}% du PASS, soit {plancher:C0}). Le taux de {tauxPourRevenusInferieursAuPass * 100} % est donc directement appliqué à ce plancher, soit {valeurPlancher:C0} de cotisations.", tauxPourRevenusInferieursAuPass, 0m);
            }
            var valeur = assiette * tauxPourRevenusInferieursAuPass;
            return new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur:C0} (PASS), donc le taux fixe de {tauxPourRevenusInferieursAuPass * 100:F2}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.", tauxPourRevenusInferieursAuPass, 0m);
        }
        else
        {
            var depassementDuPass = assiette - PASS.Valeur;

            var cotisationsSurPass = PASS.Valeur * tauxPourRevenusInferieursAuPass;
            var cotisationsSurDepassement = depassementDuPass * tauxPourRevenusSuperieursAuPass;

            var valeur = cotisationsSurPass + cotisationsSurDepassement;
            return new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur:C0} (PASS), donc le taux fixe de {tauxPourRevenusInferieursAuPass * 100:F2}% est appliqué à la part des revenus inférieure au PASS et le taux de {tauxPourRevenusSuperieursAuPass * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.", tauxPourRevenusInferieursAuPass, tauxPourRevenusSuperieursAuPass);
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
        var cotisationDeuxiemeTranche = tauxDeuxiemeTranche * (Math.Min(assiette, PASS.Valeur400Pct) - plafondsRetraiteComplementaireArtisansCommercants);

        var cotisations = cotisationPremiereTranche + cotisationDeuxiemeTranche;
        return new ResultatAvecTauxMultiplesEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {plafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {tauxPremiereTranche * 100:N0}% est appliqué à la part des revenus inférieure à cette valeur et le taux fixe de {tauxDeuxiemeTranche * 100:N0}% est appliqué à la part des revenus qui y est supérieure, soit {cotisations:C0} de cotisations.", tauxPremiereTranche, tauxDeuxiemeTranche);
    }

    public ResultatAvecTauxUniqueEtExplication CalculeLaCotisationInvaliditeDeces(decimal assiette)
    {
        var valeur = Math.Min(assiette, PASS.Valeur) * TauxInchanges.InvaliditeDeces;
        if (assiette <= PASS.Valeur)
        {
            var plancherInvaliditeDeces = TauxInchanges.PlancherInvaliditeDeces * PASS.Valeur;
            if (assiette < plancherInvaliditeDeces)
            {
                valeur = plancherInvaliditeDeces * TauxInchanges.InvaliditeDeces;
                return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure au plancher ({TauxInchanges.PlancherInvaliditeDeces * 100}% du PASS, soit {plancherInvaliditeDeces:C0}). Le taux de {TauxInchanges.InvaliditeDeces * 100} % est donc directement appliqué à ce plancher, soit {valeur:C0} de cotisations.", TauxInchanges.InvaliditeDeces);
            }

            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure ou égale au PASS ({PASS.Valeur:C0}). Le taux de {TauxInchanges.InvaliditeDeces * 100} % est donc directement appliqué à cette assiette, soit {valeur:C0} de cotisations.", TauxInchanges.InvaliditeDeces);
        }

        return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure au PASS ({PASS.Valeur:C0}). Le taux de {TauxInchanges.InvaliditeDeces * 100:F1} % est donc appliqué au PASS, soit {valeur:C0} de cotisations.", TauxInchanges.InvaliditeDeces);
    }

    public ResultatAvecTauxUniqueEtExplication CalculeLesAllocationsFamiliales(decimal assiette)
    {
        if (assiette <= PASS.Valeur110Pct)
        {
            return new ResultatAvecTauxUniqueEtExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur110Pct:C0} (110% du PASS). Il n'y a donc pas de cotisation à payer pour les allocations familiales.", 0m);
        }

        if (assiette > PASS.Valeur110Pct && assiette <= PASS.Valeur140Pct)
        {
            var difference = assiette - PASS.Valeur110Pct;
            var differenceEntrePlafondsEtPlancher = PASS.Valeur140Pct - PASS.Valeur110Pct;
            var tauxApplicable = difference / differenceEntrePlafondsEtPlancher * TauxInchanges.CotisationsAllocationsFamiliales;

            var valeur = assiette * tauxApplicable;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur140Pct:C0} (140% du PASS). Un taux progressif entre 0% et {TauxInchanges.CotisationsAllocationsFamiliales * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F2}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }
        else
        {
            var valeur = assiette * TauxInchanges.CotisationsAllocationsFamiliales;
            return new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur140Pct:C0} (140% du PASS) donc un taux fixe de {TauxInchanges.CotisationsAllocationsFamiliales * 100:F1}% est appliqué, soit {valeur:C0} de cotisations.", TauxInchanges.CotisationsAllocationsFamiliales);
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
        var valeur = PASS.Valeur * TauxInchanges.CotisationsFormationProfessionnelle;
        return new ResultatAvecTauxUniqueEtExplication(valeur, $"Un taux fixe de {TauxInchanges.CotisationsFormationProfessionnelle * 100:F2}% est appliqué sur la valeur d'un PASS complet qui vaut {PASS.Valeur:C0}, soit {valeur:C0} de cotisations.", TauxInchanges.CotisationsFormationProfessionnelle);
    }
}