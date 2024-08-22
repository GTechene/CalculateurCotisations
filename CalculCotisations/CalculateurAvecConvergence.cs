namespace CalculCotisations;

/// <summary>
/// Calcule les cotisations en faisant converger l'assiette de base (revenu net + CSG non déductible + CRDS) avec l'assiette calculée. En effet, pour calculer CSG et CRDS, il faut connaître le total des cotisations obligatoires. Or celles-ci ne sont calculables qu'en connaissant l'assiette de base... qui dépend de la CSG et de la CRDS.
/// J'ai donc opté pour un ratio "au doigt mouillé" pour le premier calcul (1.125) puis je fais converger par dichotomie en fonction de l'assiette calculée à partir de ce premier calcul.
/// </summary>
public class CalculateurAvecConvergence
{
    private readonly CalculateurSimple _calculateur;
    private readonly decimal _revenuNet;

    public CalculateurAvecConvergence(decimal revenuNet, int year = 2024)
    {
        _revenuNet = revenuNet;
        _calculateur = new CalculateurSimple(_revenuNet, year);
    }

    public decimal TotalCotisationsObligatoires => _calculateur.TotalCotisationsObligatoires;
    public decimal MaladieHorsIndemnitesJournalieres => _calculateur.MaladieHorsIndemnitesJournalieres;
    public decimal MaladieIndemnitesJournalieres => _calculateur.MaladieIndemnitesJournalieres;
    public decimal RetraiteDeBase => _calculateur.RetraiteDeBase;
    public decimal RetraiteComplementaire => _calculateur.RetraiteComplementaire;
    public decimal InvaliditeDeces => _calculateur.InvaliditeDeces;
    public decimal AllocationsFamiliales => _calculateur.AllocationsFamiliales;
    public decimal CSGNonDeductible => _calculateur.CSGNonDeductible;
    public decimal CSGDeductible => _calculateur.CSGDeductible;
    public decimal CRDS => _calculateur.CRDSNonDeductible;
    public decimal FormationProfessionnelle => _calculateur.FormationProfessionnelle;
    public decimal GrandTotal => _calculateur.GrandTotal;


    public void Calcule()
    {
        var ratioMin = 1m;
        var ratioMax = 1.25m;
        var ratio = ratioMin + (ratioMax - ratioMin) / 2;
        var assietteDeBase = _revenuNet * ratio;
        var assietteCalculee = 0m;
        var diffAssiettes = assietteCalculee - assietteDeBase;
        while (Math.Abs(diffAssiettes) > 1)
        {
            _calculateur.CalculeLesCotisations(assietteDeBase);
            assietteCalculee = _revenuNet + _calculateur.CSGNonDeductible + _calculateur.CRDSNonDeductible;
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

            assietteDeBase = _revenuNet * ratio;
        }
    }
}
