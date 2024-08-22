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
        MaladieIndemnitesJournalieres = Taux.CotisationsMaladiePourIndemnites * Math.Min(assiette, _pass.Valeur500Pct);

        CalculeLaRetraiteDeBase(assiette);
        CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(assiette);

        InvaliditeDeces = Math.Min(assiette, _pass.Valeur) * Taux.InvaliditeDeces;

        CalculeLesAllocationsFamiliales(assiette);
        CalculeCSGEtCRDS(assiette);

        FormationProfessionnelle = _pass.Valeur * Taux.CotisationsFormationProfessionnelle;
    }

    private void CalculeLesCotisationsMaladieHorsIndemnitesJournalieres(decimal assiette)
    {
        if (_revenuNet > _pass.Valeur60Pct && _revenuNet <= _pass.Valeur110Pct)
        {
            var difference = assiette - _pass.Valeur60Pct;
            var differenceEntrePlafondEtPlancher = _pass.Valeur110Pct - _pass.Valeur60Pct;
            var tauxApplicable = difference/differenceEntrePlafondEtPlancher * 0.027m + 0.04m;

            MaladieHorsIndemnitesJournalieres = tauxApplicable * assiette;
        }
        if (_revenuNet > _pass.Valeur110Pct)
        {
            MaladieHorsIndemnitesJournalieres = Taux.CotisationsMaladiePourRevenusSupAuPlancher * assiette;
        }
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
            var cotisationPremiereTranche = Taux.RetraiteComplementairePremiereTrancheArtisansCommercants * Plafonds.RetraiteComplementaireArtisansCommercants;
            var cotisationDeuxiemeTranche = Taux.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * (assiette - Plafonds.RetraiteComplementaireArtisansCommercants);

            RetraiteComplementaire = cotisationPremiereTranche + cotisationDeuxiemeTranche;
        }
    }

    private void CalculeLesAllocationsFamiliales(decimal assiette)
    {
        if (assiette > _pass.Valeur140Pct)
        {
            AllocationsFamiliales = assiette * Taux.CotisationsAllocationsFamiliales;
        }
        else if (assiette > _pass.Valeur110Pct)
        {
            var difference = assiette - _pass.Valeur110Pct;
            var differenceEntrePlafondsEtPlancher = _pass.Valeur140Pct - _pass.Valeur110Pct;
            var tauxApplicable = difference/differenceEntrePlafondsEtPlancher * Taux.CotisationsAllocationsFamiliales;

            AllocationsFamiliales = assiette * tauxApplicable;
        }
    }

    private void CalculeCSGEtCRDS(decimal assiette)
    {
        var revenusPrisEnCompte = assiette + TotalCotisationsObligatoires;

        CSGNonDeductible = revenusPrisEnCompte * Taux.CSGNonDeductible;
        CSGDeductible = revenusPrisEnCompte * Taux.CSGDeductible;
        CRDSNonDeductible = revenusPrisEnCompte * Taux.CRDSNonDeductible;
    }
}