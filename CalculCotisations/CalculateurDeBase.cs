using System.Globalization;

namespace CalculCotisations;

/// <summary>
/// Le but de cette classe est d'exposer une factory method qui appelle les méthodes virtuelles que les classes dérivées devront implémenter. Pour l'instant, seules certaines cotisations se calculent différemment d'année en année, donc seules ces méthodes sont exposées comme virtuelles pures, les autres sont implémentées ici.
/// </summary>
public abstract class CalculateurDeBase(IConstantesAvecHistorique constantesHistoriques, PlafondAnnuelSecuriteSociale pass)
{
    protected readonly IConstantesAvecHistorique ConstantesHistoriques = constantesHistoriques;
    protected readonly PlafondAnnuelSecuriteSociale PASS = pass;

    public ResultatAvecExplication MaladieHorsIndemnitesJournalieres { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication MaladieIndemnitesJournalieres { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication RetraiteDeBase { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication RetraiteComplementaire { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication InvaliditeDeces { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication AllocationsFamiliales { protected set; get; } = new ResultatVideSansExplication();

    public decimal TotalCotisationsObligatoires => MaladieHorsIndemnitesJournalieres.Valeur + MaladieIndemnitesJournalieres.Valeur + RetraiteDeBase.Valeur + RetraiteComplementaire.Valeur + InvaliditeDeces.Valeur + AllocationsFamiliales.Valeur;
    public ResultatAvecExplication CSGNonDeductible { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication CSGDeductible { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication CRDSNonDeductible { protected set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication FormationProfessionnelle { protected set; get; } = new ResultatVideSansExplication();
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
            MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS). Il n'y a donc pas de cotisation maladie à payer.");

            return;
        }

        if (assiette > PASS.Valeur40Pct && assiette <= PASS.Valeur60Pct)
        {
            var difference = assiette - PASS.Valeur40Pct;
            var differenceEntrePlafondEtPlancher = PASS.Valeur60Pct - PASS.Valeur40Pct;
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * Taux.CotisationsMaladiePourRevenusInferieursA60PctDuPass;
            var valeur = tauxApplicable * assiette;

            MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur60Pct:C0} (60% du PASS), donc un taux progressif entre 0% et {Taux.CotisationsMaladiePourRevenusInferieursA60PctDuPass * 100:N0}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.");
            return;
        }

        if (assiette > PASS.Valeur60Pct && assiette <= PASS.Valeur110Pct)
        {
            var difference = assiette - PASS.Valeur60Pct;
            var differenceEntrePlafondEtPlancher = PASS.Valeur110Pct - PASS.Valeur60Pct;
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * (ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass - Taux.CotisationsMaladiePourRevenusInferieursA60PctDuPass) + Taux.CotisationsMaladiePourRevenusInferieursA60PctDuPass;
            var valeur = tauxApplicable * assiette;

            MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur60Pct:C0} (60% du PASS) et {PASS.Valeur110Pct:C0} (110% du PASS), donc un taux progressif entre 4% et {ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.");
            return;
        }

        if (assiette > PASS.Valeur110Pct && assiette <= PASS.Valeur500Pct)
        {
            var valeur = ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * assiette;

            MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * 100:F1}% est appliqué, soit {valeur:C0} de cotisations.");

            return;
        }

        var differenceEntreAssietteEt5Pass = assiette - PASS.Valeur500Pct;
        var valeurMax = ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * PASS.Valeur500Pct + ConstantesHistoriques.CotisationsMaladiePourRevenusSupA5Pass * differenceEntreAssietteEt5Pass;

        MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(valeurMax, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {ConstantesHistoriques.CotisationsMaladiePourRevenusSupA60PctPass * 100:F1}% est appliqué à la tranche de revenus inférieure à cette valeur et le taux fixe de {ConstantesHistoriques.CotisationsMaladiePourRevenusSupA5Pass * 100:F1}% est appliqué au reste. Soit {valeurMax:C0} de cotisations.");
    }

    protected abstract void CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette);

    private void CalculeLaRetraiteDeBase(decimal assiette)
    {
        if (assiette <= PASS.Valeur)
        {
            var valeur = assiette * Taux.CotisationsRetraiteBaseRevenusInferieursAuPass;
            RetraiteDeBase = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur:C0} (PASS), donc le taux fixe de {Taux.CotisationsRetraiteBaseRevenusInferieursAuPass * 100:F2}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.");
        }
        else
        {
            var depassementDuPass = assiette - PASS.Valeur;

            var cotisationsSurPass = PASS.Valeur * Taux.CotisationsRetraiteBaseRevenusInferieursAuPass;
            var cotisationsSurDepassement = depassementDuPass * Taux.CotisationsRetraiteBaseRevenusSuperieurAuPass;

            var valeur = cotisationsSurPass + cotisationsSurDepassement;
            RetraiteDeBase = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur:C0} (PASS), donc le taux fixe de {Taux.CotisationsRetraiteBaseRevenusInferieursAuPass * 100:F2}% est appliqué à la part des revenus inférieure au PASS et le taux de {Taux.CotisationsRetraiteBaseRevenusSuperieurAuPass * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.");
        }
    }

    protected abstract void CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(decimal assiette);

    private void CalculeLaCotisationInvaliditeDeces(decimal assiette)
    {
        var valeur = Math.Min(assiette, PASS.Valeur) * Taux.InvaliditeDeces;
        if (assiette <= PASS.Valeur)
        {
            InvaliditeDeces = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est inférieure ou égale au PASS ({PASS.Valeur:C0}). Le taux de {Taux.InvaliditeDeces * 100} % est donc directement appliqué à cette assiette, soit {valeur:C0} de cotisations.");
        }
        else
        {
            InvaliditeDeces = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est supérieure au PASS ({PASS.Valeur:C0}). Le taux de {Taux.InvaliditeDeces * 100:F1} % est donc appliqué au PASS, soit {valeur:C0} de cotisations.");
        }
    }

    private void CalculeLesAllocationsFamiliales(decimal assiette)
    {
        if (assiette <= PASS.Valeur110Pct)
        {
            AllocationsFamiliales = new ResultatAvecExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur110Pct:C0} (110% du PASS). Il n'y a donc pas de cotisation à payer pour les allocations familiales.");
            return;
        }

        if (assiette > PASS.Valeur110Pct && assiette <= PASS.Valeur140Pct)
        {
            var difference = assiette - PASS.Valeur110Pct;
            var differenceEntrePlafondsEtPlancher = PASS.Valeur140Pct - PASS.Valeur110Pct;
            var tauxApplicable = difference / differenceEntrePlafondsEtPlancher * Taux.CotisationsAllocationsFamiliales;

            var valeur = assiette * tauxApplicable;
            AllocationsFamiliales = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur140Pct:C0} (140% du PASS). Un taux progressif entre 0% et {Taux.CotisationsAllocationsFamiliales * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F2}%, soit {valeur:C0} de cotisations.");
        }
        else
        {
            var valeur = assiette * Taux.CotisationsAllocationsFamiliales;
            AllocationsFamiliales = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur140Pct:C0} (140% du PASS) donc un taux fixe de {Taux.CotisationsAllocationsFamiliales * 100:F1}% est appliqué, soit {valeur:C0} de cotisations.");
        }
    }

    private void CalculeCSGEtCRDS(decimal assiette)
    {
        var revenusPrisEnCompte = assiette + TotalCotisationsObligatoires;

        var valeurCsgNonDeductible = revenusPrisEnCompte * Taux.CSGNonDeductible;
        CSGNonDeductible = new ResultatAvecExplication(valeurCsgNonDeductible, $"Un taux fixe de {Taux.CSGNonDeductible * 100:F1}% est appliqué à la somme de l'assiette retenue pour les cotisations obligatoires ({assiette:C0}) + les cotisations obligatoires elles-mêmes ({TotalCotisationsObligatoires:C0}), soit un total de {revenusPrisEnCompte:C0}. Ce qui donne une valeur de {valeurCsgNonDeductible:C0} pour la CSG non déductible.");

        var valeurCsgDeductible = revenusPrisEnCompte * Taux.CSGDeductible;
        CSGDeductible = new ResultatAvecExplication(valeurCsgDeductible, $"Un taux fixe de {Taux.CSGDeductible * 100:F1}% est appliqué à la somme de l'assiette retenue pour les cotisations obligatoires ({assiette:C0}) + les cotisations obligatoires elles-mêmes ({TotalCotisationsObligatoires:C0}), soit un total de {revenusPrisEnCompte:C0}. Ce qui donne une valeur de une valeur de {valeurCsgDeductible:C0} pour la CSG déductible.");

        var valeurCrds = revenusPrisEnCompte * Taux.CRDSNonDeductible;
        CRDSNonDeductible = new ResultatAvecExplication(valeurCrds, $"Un taux fixe de {Taux.CRDSNonDeductible * 100:F1}% est appliqué à la somme de l'assiette retenue pour les cotisations obligatoires ({assiette:C0}) + les cotisations obligatoires elles-mêmes ({TotalCotisationsObligatoires:C0}), soit un total de {revenusPrisEnCompte:C0}. Ce qui donne une valeur de {valeurCrds:C0} pour la CRDS.");
    }

    private void CalculeLaFormationProfessionnelle()
    {
        var valeur = PASS.Valeur * Taux.CotisationsFormationProfessionnelle;
        FormationProfessionnelle = new ResultatAvecExplication(valeur, $"Un taux fixe de {Taux.CotisationsFormationProfessionnelle * 100:F2}% est appliqué sur la valeur d'un PASS complet qui vaut {PASS.Valeur:C0}, soit {valeur:C0} de cotisations.");
    }
}