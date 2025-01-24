using System.Globalization;

namespace Cotisations;

/// <summary>
/// Le but de cette classe est d'exposer une factory method qui appelle les méthodes virtuelles que les classes dérivées devront implémenter. Pour l'instant, seules certaines cotisations se calculent différemment d'année en année, donc seules ces méthodes sont exposées comme virtuelles pures, les autres sont implémentées ici.
/// </summary>
public abstract class CalculateurDeBase(IConstantesAvecHistorique constantesHistoriques, PlafondAnnuelSecuriteSociale pass) : ICalculateur
{
    protected readonly IConstantesAvecHistorique ConstantesHistoriques = constantesHistoriques;
    protected readonly PlafondAnnuelSecuriteSociale PASS = pass;

    public ResultatAvecExplicationEtTaux MaladieHorsIndemnitesJournalieres { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication MaladieIndemnitesJournalieres { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecTauxMultiplesEtExplication RetraiteDeBase { protected set; get; } = new ResultatVideAvecTauxMultiplesEtSansExplication();
    public ResultatAvecTauxMultiplesEtExplication RetraiteComplementaire { protected set; get; } = new ResultatVideAvecTauxMultiplesEtSansExplication();
    public ResultatAvecTauxUniqueEtExplication InvaliditeDeces { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication AllocationsFamiliales { protected set; get; } = new ResultatVideSansExplication();

    public decimal TotalCotisationsObligatoires => MaladieHorsIndemnitesJournalieres.Valeur + MaladieIndemnitesJournalieres.Valeur + RetraiteDeBase.Valeur + RetraiteComplementaire.Valeur + InvaliditeDeces.Valeur + AllocationsFamiliales.Valeur;
    public ResultatAvecTauxUniqueEtExplication CSGNonDeductible { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication CSGDeductible { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication CRDSNonDeductible { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecTauxUniqueEtExplication FormationProfessionnelle { protected set; get; } = new ResultatVideSansExplication();
    public decimal GrandTotal => TotalCotisationsObligatoires + CSGDeductible.Valeur + CSGNonDeductible.Valeur + CRDSNonDeductible.Valeur + FormationProfessionnelle.Valeur;

    public void CalculeLesCotisations(decimal assiette)
    {
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");

        CalculeLesCotisationsMaladieHorsIndemnitesJournalieres(assiette);
        CalculeLesCotisationsPourIndemnitesMaladie(assiette);

        CalculeLaRetraiteDeBase(assiette);
        CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(assiette);

        CalculeLaCotisationInvaliditeDeces(assiette);

        CalculeLesAllocationsFamiliales(assiette);

        CalculeCSGEtCRDS(assiette);

        CalculeLaFormationProfessionnelle();
    }

    private void CalculeLesCotisationsMaladieHorsIndemnitesJournalieres(decimal assiette)
    {
        if (assiette <= PASS.Valeur40Pct)
        {
            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS). Il n'y a donc pas de cotisation maladie à payer.", 0m);

            return;
        }

        if (assiette > PASS.Valeur40Pct && assiette <= PASS.Valeur60Pct)
        {
            var difference = assiette - PASS.Valeur40Pct;
            var differenceEntrePlafondEtPlancher = PASS.Valeur60Pct - PASS.Valeur40Pct;
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * TauxInchanges.CotisationsMaladiePourRevenusInferieursA60PctDuPass;
            var valeur = tauxApplicable * assiette;

            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur60Pct:C0} (60% du PASS), donc un taux progressif entre 0% et {TauxInchanges.CotisationsMaladiePourRevenusInferieursA60PctDuPass * 100:N0}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
            return;
        }

        if (assiette > PASS.Valeur60Pct && assiette <= PASS.Valeur110Pct)
        {
            var difference = assiette - PASS.Valeur60Pct;
            var differenceEntrePlafondEtPlancher = PASS.Valeur110Pct - PASS.Valeur60Pct;
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * (ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass - TauxInchanges.CotisationsMaladiePourRevenusInferieursA60PctDuPass) + TauxInchanges.CotisationsMaladiePourRevenusInferieursA60PctDuPass;
            var valeur = tauxApplicable * assiette;

            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur60Pct:C0} (60% du PASS) et {PASS.Valeur110Pct:C0} (110% du PASS), donc un taux progressif entre 4% et {ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.", tauxApplicable);
            return;
        }

        if (assiette > PASS.Valeur110Pct && assiette <= PASS.Valeur500Pct)
        {
            var valeur = ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * assiette;

            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * 100:F1}% est appliqué, soit {valeur:C0} de cotisations.", ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass);

            return;
        }

        var differenceEntreAssietteEt5Pass = assiette - PASS.Valeur500Pct;
        var valeurMax = ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * PASS.Valeur500Pct + ConstantesHistoriques.CotisationsMaladiePourRevenusSupA5Pass * differenceEntreAssietteEt5Pass;

        MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(valeurMax, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * 100:F1}% est appliqué à la tranche de revenus inférieure à cette valeur et le taux fixe de {ConstantesHistoriques.CotisationsMaladiePourRevenusSupA5Pass * 100:F1}% est appliqué au reste. Soit {valeurMax:C0} de cotisations.", ConstantesHistoriques.CotisationsMaladiePourRevenusSupA5Pass);
    }

    protected abstract void CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette);

    private void CalculeLaRetraiteDeBase(decimal assiette)
    {
        if (assiette <= PASS.Valeur)
        {
            var valeur = assiette * TauxInchanges.CotisationsRetraiteBaseRevenusInferieursAuPass;
            RetraiteDeBase = new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur:C0} (PASS), donc le taux fixe de {TauxInchanges.CotisationsRetraiteBaseRevenusInferieursAuPass * 100:F2}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.", TauxInchanges.CotisationsRetraiteBaseRevenusInferieursAuPass, 0m);
        }
        else
        {
            var depassementDuPass = assiette - PASS.Valeur;

            var cotisationsSurPass = PASS.Valeur * TauxInchanges.CotisationsRetraiteBaseRevenusInferieursAuPass;
            var cotisationsSurDepassement = depassementDuPass * TauxInchanges.CotisationsRetraiteBaseRevenusSuperieursAuPass;

            var valeur = cotisationsSurPass + cotisationsSurDepassement;
            RetraiteDeBase = new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur:C0} (PASS), donc le taux fixe de {TauxInchanges.CotisationsRetraiteBaseRevenusInferieursAuPass * 100:F2}% est appliqué à la part des revenus inférieure au PASS et le taux de {TauxInchanges.CotisationsRetraiteBaseRevenusSuperieursAuPass * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.", TauxInchanges.CotisationsRetraiteBaseRevenusInferieursAuPass, TauxInchanges.CotisationsRetraiteBaseRevenusSuperieursAuPass);
        }
    }

    protected abstract void CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(decimal assiette);

    private void CalculeLaCotisationInvaliditeDeces(decimal assiette)
    {
        var valeur = Math.Min(assiette, PASS.Valeur) * TauxInchanges.InvaliditeDeces;
        if (assiette <= PASS.Valeur)
        {
            InvaliditeDeces = new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure ou égale au PASS ({PASS.Valeur:C0}). Le taux de {TauxInchanges.InvaliditeDeces * 100} % est donc directement appliqué à cette assiette, soit {valeur:C0} de cotisations.", TauxInchanges.InvaliditeDeces);
        }
        else
        {
            InvaliditeDeces = new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure au PASS ({PASS.Valeur:C0}). Le taux de {TauxInchanges.InvaliditeDeces * 100:F1} % est donc appliqué au PASS, soit {valeur:C0} de cotisations.", TauxInchanges.InvaliditeDeces);
        }
    }

    private void CalculeLesAllocationsFamiliales(decimal assiette)
    {
        if (assiette <= PASS.Valeur110Pct)
        {
            AllocationsFamiliales = new ResultatAvecTauxUniqueEtExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur110Pct:C0} (110% du PASS). Il n'y a donc pas de cotisation à payer pour les allocations familiales.", 0m);
            return;
        }

        if (assiette > PASS.Valeur110Pct && assiette <= PASS.Valeur140Pct)
        {
            var difference = assiette - PASS.Valeur110Pct;
            var differenceEntrePlafondsEtPlancher = PASS.Valeur140Pct - PASS.Valeur110Pct;
            var tauxApplicable = difference / differenceEntrePlafondsEtPlancher * TauxInchanges.CotisationsAllocationsFamiliales;

            var valeur = assiette * tauxApplicable;
            AllocationsFamiliales = new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur140Pct:C0} (140% du PASS). Un taux progressif entre 0% et {TauxInchanges.CotisationsAllocationsFamiliales * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F2}%, soit {valeur:C0} de cotisations.", tauxApplicable);
        }
        else
        {
            var valeur = assiette * TauxInchanges.CotisationsAllocationsFamiliales;
            AllocationsFamiliales = new ResultatAvecTauxUniqueEtExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur140Pct:C0} (140% du PASS) donc un taux fixe de {TauxInchanges.CotisationsAllocationsFamiliales * 100:F1}% est appliqué, soit {valeur:C0} de cotisations.", TauxInchanges.CotisationsAllocationsFamiliales);
        }
    }

    private void CalculeCSGEtCRDS(decimal assiette)
    {
        var revenusPrisEnCompte = assiette + TotalCotisationsObligatoires;

        var valeurCsgNonDeductible = revenusPrisEnCompte * TauxInchanges.CSGNonDeductible;
        CSGNonDeductible = new ResultatAvecTauxUniqueEtExplication(valeurCsgNonDeductible, $"L'assiette de calcul de la CSG est égale à l'assiette retenue pour les cotisations obligatoires ({assiette:C0}) + les cotisations obligatoires elles-mêmes ({TotalCotisationsObligatoires:C0}), soit un total de {revenusPrisEnCompte:C0}. Le taux fixe de {TauxInchanges.CSGNonDeductible * 100:F1}% est appliqué à cette assiette, ce qui donne une valeur de {valeurCsgNonDeductible:C0} pour la CSG non déductible.", TauxInchanges.CSGNonDeductible);

        var valeurCsgDeductible = revenusPrisEnCompte * TauxInchanges.CSGDeductible;
        CSGDeductible = new ResultatAvecTauxUniqueEtExplication(valeurCsgDeductible, $"L'assiette de calcul de la CSG est égale à l'assiette retenue pour les cotisations obligatoires ({assiette:C0}) + les cotisations obligatoires elles-mêmes ({TotalCotisationsObligatoires:C0}), soit un total de {revenusPrisEnCompte:C0}. Le taux fixe de {TauxInchanges.CSGDeductible * 100:F1}% est appliqué à cette assiette, ce qui donne une valeur de une valeur de {valeurCsgDeductible:C0} pour la CSG déductible.", TauxInchanges.CSGDeductible);

        var valeurCrds = revenusPrisEnCompte * TauxInchanges.CRDSNonDeductible;
        CRDSNonDeductible = new ResultatAvecTauxUniqueEtExplication(valeurCrds, $"L'assiette de calcul de la CRDS est égale à l'assiette retenue pour les cotisations obligatoires ({assiette:C0}) + les cotisations obligatoires elles-mêmes ({TotalCotisationsObligatoires:C0}), soit un total de {revenusPrisEnCompte:C0}. Le taux fixe de {TauxInchanges.CRDSNonDeductible * 100:F1}% est appliqué à cette assiette, ce qui donne une valeur de {valeurCrds:C0} pour la CRDS.", TauxInchanges.CRDSNonDeductible);
    }

    private void CalculeLaFormationProfessionnelle()
    {
        var valeur = PASS.Valeur * TauxInchanges.CotisationsFormationProfessionnelle;
        FormationProfessionnelle = new ResultatAvecTauxUniqueEtExplication(valeur, $"Un taux fixe de {TauxInchanges.CotisationsFormationProfessionnelle * 100:F2}% est appliqué sur la valeur d'un PASS complet qui vaut {PASS.Valeur:C0}, soit {valeur:C0} de cotisations.", TauxInchanges.CotisationsFormationProfessionnelle);
    }
}

public class CalculateurCommun(PlafondAnnuelSecuriteSociale pass)
{
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

    public (ResultatAvecTauxUniqueEtExplication csgNonDeductible, ResultatAvecTauxUniqueEtExplication csgDeductible, ResultatAvecTauxUniqueEtExplication crdsNonDeductible)
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