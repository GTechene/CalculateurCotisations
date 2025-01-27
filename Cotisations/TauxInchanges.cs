namespace Cotisations;

/// <summary>
/// Contient tous les taux inchangés pour l'instant.
/// Taux appliqués selon la doc officielle de l'URSSAF pour les professions libérales non réglementées.
/// Voir https://www.urssaf.fr/accueil/outils-documentation/taux-baremes/taux-cotisations-ac-plnr.html
/// </summary>
public static class TauxInchanges
{
    public const decimal CotisationsMaladiePourRevenusInferieursA60PctDuPass = 0.04m;
    public const decimal InvaliditeDeces = 0.013m;
    public const decimal PlancherInvaliditeDeces = 0.115m;
    public const decimal PlancherRetraiteDeBase = 0.115m;
    public const decimal CotisationsAllocationsFamiliales = 0.031m;
    public const decimal CSGNonDeductible = 0.024m;
    public const decimal CSGDeductible = 0.068m;
    public const decimal CRDSNonDeductible = 0.005m;
    public const decimal CotisationsFormationProfessionnelle = 0.0025m;

    // TODO : ça a bougé en 2025, déplacer dans ConstantesAvecHistorique
    public const decimal CotisationsRetraiteBaseRevenusSuperieursAuPass = 0.006m;
    public const decimal CotisationsRetraiteBaseRevenusInferieursAuPass = 0.1775m;
    public const decimal RetraiteComplementairePremiereTrancheArtisansCommercants = 0.07m;
    public const decimal RetraiteComplementaireDeuxiemeTrancheArtisansCommercants = 0.08m;
}