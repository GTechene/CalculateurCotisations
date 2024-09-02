using System.Globalization;

namespace CalculCotisations;

/// <summary>
/// Calculateur simple des cotisations. Les cotisations obligatoires sont calculées à partir du montant fourni et la CSG et la CRDS sont calculées à partir de ces cotisations, ce qui peut être incorrect. Pour un calculateur plus précis, voir <see cref="CalculateurAvecConvergence"/>.
/// </summary>
public class CalculateurSimple
{
    private readonly decimal _revenuNet;
    private readonly PlafondAnnuelSecuriteSociale _pass;
    public ResultatAvecExplication MaladieHorsIndemnitesJournalieres { private set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication MaladieIndemnitesJournalieres { private set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication RetraiteDeBase { private set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication RetraiteComplementaire { private set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication InvaliditeDeces { private set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication AllocationsFamiliales { private set; get; } = new ResultatVideSansExplication();

    public decimal TotalCotisationsObligatoires => MaladieHorsIndemnitesJournalieres.Valeur + MaladieIndemnitesJournalieres.Valeur + RetraiteDeBase.Valeur + RetraiteComplementaire.Valeur + InvaliditeDeces.Valeur + AllocationsFamiliales.Valeur;
    public ResultatAvecExplication CSGNonDeductible { private set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication CSGDeductible { private set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication CRDSNonDeductible { private set; get; } = new ResultatVideSansExplication();
    public ResultatAvecExplication FormationProfessionnelle { private set; get; } = new ResultatVideSansExplication();
    public decimal GrandTotal => TotalCotisationsObligatoires + CSGDeductible.Valeur + CSGNonDeductible.Valeur + CRDSNonDeductible.Valeur + FormationProfessionnelle.Valeur;

    public CalculateurSimple(decimal revenuNet, int year = 2024)
    {
        _revenuNet = revenuNet;
        _pass = new PlafondAnnuelSecuriteSociale(year);
        CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
    }

    public void CalculeLesCotisations()
    {
        CalculeLesCotisations(_revenuNet);
    }

    public void CalculeLesCotisations(decimal assiette)
    {
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
        if (assiette <= _pass.Valeur40Pct)
        {
            MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {_pass.Valeur40Pct:C0} (40% du PASS). Il n'y a donc pas de cotisation maladie à payer.");
            return;
        }

        if (assiette > _pass.Valeur40Pct && assiette <= _pass.Valeur60Pct)
        {
            var difference = assiette - _pass.Valeur40Pct;
            var differenceEntrePlafondEtPlancher = _pass.Valeur60Pct - _pass.Valeur40Pct;
            // TODO : utiliser une constante dans la classe Taux
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * 0.04m;
            var valeur = tauxApplicable * assiette;

            MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {_pass.Valeur40Pct:C0} (40% du PASS) et {_pass.Valeur60Pct:C0} (60% du PASS), donc un taux progressif entre 0% et 4% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.");
            return;
        }

        if (assiette > _pass.Valeur60Pct && assiette <= _pass.Valeur110Pct)
        {
            var difference = assiette - _pass.Valeur60Pct;
            var differenceEntrePlafondEtPlancher = _pass.Valeur110Pct - _pass.Valeur60Pct;
            // TODO : utiliser une constante dans la classe Taux ainsi que la diff qui va bien pour avoir les 0.027
            var tauxApplicable = difference/differenceEntrePlafondEtPlancher * 0.027m + 0.04m;
            var valeur = tauxApplicable * assiette;

            MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {_pass.Valeur60Pct:C0} (60% du PASS) et {_pass.Valeur110Pct:C0} (110% du PASS), donc un taux progressif entre 4% et {Taux.CotisationsMaladiePourRevenusSupAuPlancher * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.");
            return;
        }

        if (assiette > _pass.Valeur110Pct && assiette <= _pass.Valeur500Pct)
        {
            var valeur = Taux.CotisationsMaladiePourRevenusSupAuPlancher * assiette;

            MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {_pass.Valeur110Pct:C0} (110% du PASS) et {_pass.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {Taux.CotisationsMaladiePourRevenusSupAuPlancher * 100:F1}% est appliqué, soit {valeur:C0} de cotisations.");

            return;
        }

        var differenceEntreAssietteEt5Pass = assiette - _pass.Valeur500Pct;
        var valeurMax = Taux.CotisationsMaladiePourRevenusSupAuPlancher * _pass.Valeur500Pct + Taux.CotisationsMaladiePourRevenusSupA5Pass * differenceEntreAssietteEt5Pass;

        MaladieHorsIndemnitesJournalieres = new ResultatAvecExplication(valeurMax, $"L'assiette de {assiette:C0} est supérieure à {_pass.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {Taux.CotisationsMaladiePourRevenusSupAuPlancher * 100:F1}% est appliqué à la tranche de revenus inférieure à cette valeur et le taux fixe de {Taux.CotisationsMaladiePourRevenusSupA5Pass * 100:F1}% est appliqué au reste. Soit {valeurMax:C0} de cotisations.");
    }

    private void CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette)
    {
        if (assiette < _pass.Valeur40Pct)
        {
            var cotisations = Taux.CotisationsMaladiePourIndemnites * _pass.Valeur40Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecExplication(cotisations, $"L'assiette de {assiette:C0} est inférieure à {_pass.Valeur40Pct:C0} (40% du PASS), donc le taux fixe de {Taux.CotisationsMaladiePourIndemnites * 100:F1}% est appliqué à ce plancher de 40% du PASS, soit {cotisations:C0} de cotisations.");
            return;
        }

        if (assiette < _pass.Valeur500Pct)
        {
            var cotisations = Taux.CotisationsMaladiePourIndemnites * assiette;
            MaladieIndemnitesJournalieres = new ResultatAvecExplication(cotisations, $"L'assiette de {assiette:C0} est comprise entre {_pass.Valeur40Pct:C0} (40% du PASS) et {_pass.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {Taux.CotisationsMaladiePourIndemnites * 100:F1}% est appliqué à cette assiette, soit {cotisations:C0} de cotisations.");
        }
        else
        {
            var cotisations = Taux.CotisationsMaladiePourIndemnites * _pass.Valeur500Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {_pass.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {Taux.CotisationsMaladiePourIndemnites * 100:F1}% est appliqué à ce plafond de 500% du PASS, soit {cotisations:C0} de cotisations.");
        }
    }

    private void CalculeLaRetraiteDeBase(decimal assiette)
    {
        if (assiette <= _pass.Valeur)
        {
            var valeur = assiette * Taux.CotisationsRetraiteBaseRevenusInferieursAuPass;
            RetraiteDeBase = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {_pass.Valeur:C0} (PASS), donc le taux fixe de {Taux.CotisationsRetraiteBaseRevenusInferieursAuPass * 100:F2}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.");
        }
        else
        {
            var depassementDuPass = assiette - _pass.Valeur;

            var cotisationsSurPass = _pass.Valeur * Taux.CotisationsRetraiteBaseRevenusInferieursAuPass;
            var cotisationsSurDepassement = depassementDuPass * Taux.CotisationsRetraiteBaseRevenusSuperieurAuPass;

            var valeur = cotisationsSurPass + cotisationsSurDepassement;
            RetraiteDeBase = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {_pass.Valeur:C0} (PASS), donc le taux fixe de {Taux.CotisationsRetraiteBaseRevenusInferieursAuPass * 100:F2}% est appliqué à la part des revenus inférieure au PASS et le taux de {Taux.CotisationsRetraiteBaseRevenusSuperieurAuPass * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.");
        }
    }

    private void CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(decimal assiette)
    {
        if (assiette <= Plafonds.RetraiteComplementaireArtisansCommercants)
        {
            var valeur = assiette * Taux.RetraiteComplementairePremiereTrancheArtisansCommercants;
            RetraiteComplementaire = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {Plafonds.RetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {Taux.RetraiteComplementairePremiereTrancheArtisansCommercants * 100:N0}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.");
            ;
        }
        else
        {
            const decimal cotisationPremiereTranche = Taux.RetraiteComplementairePremiereTrancheArtisansCommercants * Plafonds.RetraiteComplementaireArtisansCommercants;
            var cotisationDeuxiemeTranche = Taux.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * (Math.Min(assiette, _pass.Valeur400Pct) - Plafonds.RetraiteComplementaireArtisansCommercants);

            var cotisations = cotisationPremiereTranche + cotisationDeuxiemeTranche;
            RetraiteComplementaire = new ResultatAvecExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {Plafonds.RetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {Taux.RetraiteComplementairePremiereTrancheArtisansCommercants * 100:N0}% est appliqué à la part des revenus inférieure à cette valeur et le taux fixe de {Taux.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * 100:N0}% est appliqué à la part des revenus qui y est supérieure, soit {cotisations:C0} de cotisations.");
        }
    }

    private void CalculeLaCotisationInvaliditeDeces(decimal assiette)
    {
        var valeur = Math.Min(assiette, _pass.Valeur) * Taux.InvaliditeDeces;
        if (assiette <= _pass.Valeur)
        {
            InvaliditeDeces = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est inférieure ou égale au PASS ({_pass.Valeur:C0}). Le taux de {Taux.InvaliditeDeces * 100} % est donc directement appliqué à cette assiette, soit {valeur:C0} de cotisations.");
        }
        else
        {
            InvaliditeDeces = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est supérieure au PASS ({_pass.Valeur:C0}). Le taux de {Taux.InvaliditeDeces * 100:F1} % est donc appliqué au PASS, soit {valeur:C0} de cotisations.");
        }
    }

    private void CalculeLesAllocationsFamiliales(decimal assiette)
    {
        if (assiette <= _pass.Valeur110Pct)
        {
            AllocationsFamiliales = new ResultatAvecExplication(0m, $"L'assiette de {assiette:C0} est inférieure à {_pass.Valeur110Pct:C0} (110% du PASS). Il n'y a donc pas de cotisation à payer pour les allocations familiales.");
            return;
        }

        if (assiette > _pass.Valeur110Pct && assiette <= _pass.Valeur140Pct)
        {
            var difference = assiette - _pass.Valeur110Pct;
            var differenceEntrePlafondsEtPlancher = _pass.Valeur140Pct - _pass.Valeur110Pct;
            var tauxApplicable = difference/differenceEntrePlafondsEtPlancher * Taux.CotisationsAllocationsFamiliales;

            var valeur = assiette * tauxApplicable;
            AllocationsFamiliales = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est comprise entre {_pass.Valeur110Pct:C0} (110% du PASS) et {_pass.Valeur140Pct:C0} (140% du PASS). Un taux progressif entre 0% et {Taux.CotisationsAllocationsFamiliales * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F2}%, soit {valeur:C0} de cotisations.");
        }
        else
        {
            var valeur = assiette * Taux.CotisationsAllocationsFamiliales;
            AllocationsFamiliales = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est supérieure à {_pass.Valeur140Pct:C0} (140% du PASS) donc un taux fixe de {Taux.CotisationsAllocationsFamiliales * 100:F1}% est appliqué, soit {valeur:C0} de cotisations.");
        }
    }

    private void CalculeCSGEtCRDS(decimal assiette)
    {
        var revenusPrisEnCompte = assiette + TotalCotisationsObligatoires;

        var valeurCsgNonDeductible = revenusPrisEnCompte * Taux.CSGNonDeductible;
        CSGNonDeductible = new ResultatAvecExplication(valeurCsgNonDeductible, $"Un taux fixe de {Taux.CSGNonDeductible * 100:F1}% est appliqué à la somme des revenus + les cotisations obligatoires. Soit une valeur de {valeurCsgNonDeductible:C0} pour la CSG non déductible.");

        var valeurCsgDeductible = revenusPrisEnCompte * Taux.CSGDeductible;
        CSGDeductible = new ResultatAvecExplication(valeurCsgDeductible, $"Un taux fixe de {Taux.CSGDeductible * 100:F1}% est appliqué à la somme des revenus + les cotisations obligatoires. Soit une valeur de {valeurCsgDeductible:C0} pour la CSG déductible.");

        var valeurCrds = revenusPrisEnCompte * Taux.CRDSNonDeductible;
        CRDSNonDeductible = new ResultatAvecExplication(valeurCrds, $"Un taux fixe de {Taux.CRDSNonDeductible * 100:F1}% est appliqué à la somme des revenus + les cotisations obligatoires. Soit une valeur de {valeurCrds:C0} pour la CRDS.");
    }

    private void CalculeLaFormationProfessionnelle()
    {
        var valeur = _pass.Valeur * Taux.CotisationsFormationProfessionnelle;
        FormationProfessionnelle = new ResultatAvecExplication(valeur, $"Le taux de {Taux.CotisationsFormationProfessionnelle * 100:F2} est appliqué sur la valeur d'un PASS complet qui vaut {_pass.Valeur:C0}, soit {valeur:C0} de cotisations.");
    }
}