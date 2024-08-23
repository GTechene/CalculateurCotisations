namespace CalculCotisations;

/// <summary>
/// Calculateur simple des cotisations. Les cotisations obligatoires sont calculées à partir du montant fourni et la CSG et la CRDS sont calculées à partir de ces cotisations, ce qui peut être incorrect. Pour un calculateur plus précis, voir <see cref="CalculateurAvecConvergence"/>.
/// </summary>
public class CalculateurSimple
{
    private readonly decimal _revenuNet;
    private readonly PlafondAnnuelSecuriteSociale _pass;
    public decimal MaladieHorsIndemnitesJournalieres { private set; get; }
    public decimal MaladieIndemnitesJournalieres { private set; get; }
    public decimal RetraiteDeBase { private set; get; }
    public decimal RetraiteComplementaire { private set; get; }
    public decimal InvaliditeDeces { private set; get; }
    public decimal AllocationsFamiliales { private set; get; }

    public decimal TotalCotisationsObligatoires => MaladieHorsIndemnitesJournalieres + MaladieIndemnitesJournalieres + RetraiteDeBase + RetraiteComplementaire + InvaliditeDeces + AllocationsFamiliales;
    public decimal CSGNonDeductible { private set; get; }
    public decimal CSGDeductible { private set; get; }
    public decimal CRDSNonDeductible { private set; get; }
    public decimal FormationProfessionnelle { private set; get; }
    public decimal GrandTotal => TotalCotisationsObligatoires + CSGDeductible + CSGNonDeductible + CRDSNonDeductible + FormationProfessionnelle;

    public CalculateurSimple(decimal revenuNet, int year = 2024)
    {
        _revenuNet = revenuNet;
        _pass = new PlafondAnnuelSecuriteSociale(year);
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

        InvaliditeDeces = Math.Min(assiette, _pass.Valeur) * Taux.InvaliditeDeces;

        CalculeLesAllocationsFamiliales(assiette);
        CalculeCSGEtCRDS(assiette);

        FormationProfessionnelle = _pass.Valeur * Taux.CotisationsFormationProfessionnelle;
    }

    private void CalculeLesCotisationsMaladieHorsIndemnitesJournalieres(decimal assiette)
    {
        if (_revenuNet <= _pass.Valeur40Pct)
        {
            return;
        }

        if (_revenuNet > _pass.Valeur40Pct && _revenuNet <= _pass.Valeur60Pct)
        {
            var difference = assiette - _pass.Valeur40Pct;
            var differenceEntrePlafondEtPlancher = _pass.Valeur60Pct - _pass.Valeur40Pct;
            var tauxApplicable = difference / differenceEntrePlafondEtPlancher * 0.04m;

            MaladieHorsIndemnitesJournalieres = tauxApplicable * assiette;
            return;
        }

        if (_revenuNet > _pass.Valeur60Pct && _revenuNet <= _pass.Valeur110Pct)
        {
            var difference = assiette - _pass.Valeur60Pct;
            var differenceEntrePlafondEtPlancher = _pass.Valeur110Pct - _pass.Valeur60Pct;
            var tauxApplicable = difference/differenceEntrePlafondEtPlancher * 0.027m + 0.04m;

            MaladieHorsIndemnitesJournalieres = tauxApplicable * assiette;
            return;
        }

        if (_revenuNet > _pass.Valeur110Pct && _revenuNet <= _pass.Valeur500Pct)
        {
            MaladieHorsIndemnitesJournalieres = Taux.CotisationsMaladiePourRevenusSupAuPlancher * assiette;
            return;
        }

        var differenceEntreAssietteEt5Pass = assiette - _pass.Valeur500Pct;
        MaladieHorsIndemnitesJournalieres = Taux.CotisationsMaladiePourRevenusSupAuPlancher * _pass.Valeur500Pct + Taux.CotisationsMaladiePourRevenusSupA5Pass * differenceEntreAssietteEt5Pass;
    }

    private void CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette)
    {
        if (assiette < _pass.Valeur40Pct)
        {
            MaladieIndemnitesJournalieres = Taux.CotisationsMaladiePourIndemnites * _pass.Valeur40Pct;
            return;
        }

        MaladieIndemnitesJournalieres = Taux.CotisationsMaladiePourIndemnites * Math.Min(assiette, _pass.Valeur500Pct);
    }

    private void CalculeLaRetraiteDeBase(decimal assiette)
    {
        if (assiette <= _pass.Valeur)
        {
            RetraiteDeBase = assiette * Taux.CotisationsRetraiteBaseRevenusInferieursAuPass;
        }
        else
        {
            var depassementDuPass = assiette - _pass.Valeur;

            var cotisationsSurPass = _pass.Valeur * Taux.CotisationsRetraiteBaseRevenusInferieursAuPass;
            var cotisationsSurDepassement = depassementDuPass * Taux.CotisationsRetraiteBaseRevenusSuperieurAuPass;

            RetraiteDeBase = cotisationsSurPass + cotisationsSurDepassement;
        }
    }

    private void CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(decimal assiette)
    {
        if (assiette <= Plafonds.RetraiteComplementaireArtisansCommercants)
        {
            RetraiteComplementaire = assiette * Taux.RetraiteComplementairePremiereTrancheArtisansCommercants;
        }
        else
        {
            const decimal cotisationPremiereTranche = Taux.RetraiteComplementairePremiereTrancheArtisansCommercants * Plafonds.RetraiteComplementaireArtisansCommercants;
            var cotisationDeuxiemeTranche = Taux.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * (Math.Min(assiette, _pass.Valeur400Pct) - Plafonds.RetraiteComplementaireArtisansCommercants);

            RetraiteComplementaire = cotisationPremiereTranche + cotisationDeuxiemeTranche;
        }
    }

    private void CalculeLesAllocationsFamiliales(decimal assiette)
    {
        if (assiette <= _pass.Valeur110Pct)
        {
            return;
        }

        if (assiette > _pass.Valeur110Pct && assiette <= _pass.Valeur140Pct)
        {
            var difference = assiette - _pass.Valeur110Pct;
            var differenceEntrePlafondsEtPlancher = _pass.Valeur140Pct - _pass.Valeur110Pct;
            var tauxApplicable = difference/differenceEntrePlafondsEtPlancher * Taux.CotisationsAllocationsFamiliales;

            AllocationsFamiliales = assiette * tauxApplicable;
            return;
        }

        AllocationsFamiliales = assiette * Taux.CotisationsAllocationsFamiliales;
    }

    private void CalculeCSGEtCRDS(decimal assiette)
    {
        var revenusPrisEnCompte = assiette + TotalCotisationsObligatoires;

        CSGNonDeductible = revenusPrisEnCompte * Taux.CSGNonDeductible;
        CSGDeductible = revenusPrisEnCompte * Taux.CSGDeductible;
        CRDSNonDeductible = revenusPrisEnCompte * Taux.CRDSNonDeductible;
    }
}