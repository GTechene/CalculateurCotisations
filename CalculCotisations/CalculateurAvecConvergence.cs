namespace CalculCotisations;

/// <summary>
/// Calcule les cotisations en faisant converger l'assiette de base (revenu net + CSG non déductible + CRDS) avec l'assiette calculée. En effet, pour calculer CSG et CRDS, il faut connaître le total des cotisations obligatoires. Or celles-ci ne sont calculables qu'en connaissant l'assiette de base... qui dépend de la CSG et de la CRDS.
/// J'ai donc opté pour un ratio "au doigt mouillé" pour le premier calcul (1.125) puis je fais converger par dichotomie en fonction de l'assiette calculée à partir de ce premier calcul.
/// </summary>
public class CalculateurAvecConvergence(decimal revenuNet, int year = 2024)
{
    private readonly CalculateurDeBase _calculateur = Calculateurs.TrouveUnCalculateur(year);

    public decimal TotalCotisationsObligatoires => _calculateur.TotalCotisationsObligatoires;
    public ResultatAvecExplication MaladieHorsIndemnitesJournalieres => _calculateur.MaladieHorsIndemnitesJournalieres;
    public ResultatAvecExplication MaladieIndemnitesJournalieres => _calculateur.MaladieIndemnitesJournalieres;
    public ResultatAvecExplication RetraiteDeBase => _calculateur.RetraiteDeBase;
    public ResultatAvecExplication RetraiteComplementaire => _calculateur.RetraiteComplementaire;
    public ResultatAvecExplication InvaliditeDeces => _calculateur.InvaliditeDeces;
    public ResultatAvecExplication AllocationsFamiliales => _calculateur.AllocationsFamiliales;
    public ResultatAvecExplication CSGNonDeductible => _calculateur.CSGNonDeductible;
    public ResultatAvecExplication CSGDeductible => _calculateur.CSGDeductible;
    public ResultatAvecExplication CRDS => _calculateur.CRDSNonDeductible;
    public ResultatAvecExplication FormationProfessionnelle => _calculateur.FormationProfessionnelle;
    public decimal GrandTotal => _calculateur.GrandTotal;


    public void Calcule()
    {
        var ratioMin = 1m;
        var ratioMax = 1.25m;
        var ratio = ratioMin + (ratioMax - ratioMin) / 2;
        var assietteDeBase = revenuNet * ratio;
        var assietteCalculee = 0m;
        var diffAssiettes = assietteCalculee - assietteDeBase;
        while (Math.Abs(diffAssiettes) > 1)
        {
            _calculateur.CalculeLesCotisations(assietteDeBase);
            assietteCalculee = revenuNet + _calculateur.CSGNonDeductible.Valeur + _calculateur.CRDSNonDeductible.Valeur;
            diffAssiettes = assietteCalculee - assietteDeBase;
            if (assietteCalculee <= assietteDeBase)
            {
                ratioMax = ratio;
                ratio -= (ratioMax - ratioMin) / 2;
            }
            else
            {
                ratioMin = ratio;
                ratio += (ratioMax - ratioMin) / 2;
            }

            assietteDeBase = revenuNet * ratio;
        }
    }
}
