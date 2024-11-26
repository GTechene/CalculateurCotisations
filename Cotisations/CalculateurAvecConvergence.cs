namespace Cotisations;

/// <summary>
/// Calcule les cotisations en faisant converger l'assiette de base (revenu net + CSG non déductible + CRDS) avec l'assiette calculée. En effet, pour calculer CSG et CRDS, il faut connaître le total des cotisations obligatoires. Or celles-ci ne sont calculables qu'en connaissant l'assiette de base... qui dépend de la CSG et de la CRDS.
/// J'ai donc opté pour un ratio "au doigt mouillé" pour le premier calcul (1.125) puis je fais converger par dichotomie en fonction de l'assiette calculée à partir de ce premier calcul.
/// </summary>
public class CalculateurAvecConvergence(decimal revenuNet, int annee = 2024, decimal cotisationsFacultatives = 0m)
{
    private readonly CalculateurDeBase _calculateur = Calculateurs.TrouveUnCalculateur(annee);
    private const int NombreDIterationsMaximal = 100;

    public decimal TotalCotisationsObligatoires => _calculateur.TotalCotisationsObligatoires;
    public ResultatAvecTauxEtExplication MaladieHorsIndemnitesJournalieres => _calculateur.MaladieHorsIndemnitesJournalieres;
    public ResultatAvecTauxEtExplication MaladieIndemnitesJournalieres => _calculateur.MaladieIndemnitesJournalieres;
    public ResultatAvecTauxMultiplesEtExplication RetraiteDeBase => _calculateur.RetraiteDeBase;
    public ResultatAvecTauxMultiplesEtExplication RetraiteComplementaire => _calculateur.RetraiteComplementaire;
    public ResultatAvecTauxEtExplication InvaliditeDeces => _calculateur.InvaliditeDeces;
    public ResultatAvecTauxEtExplication AllocationsFamiliales => _calculateur.AllocationsFamiliales;
    public ResultatAvecTauxEtExplication CSGNonDeductible => _calculateur.CSGNonDeductible;
    public ResultatAvecTauxEtExplication CSGDeductible => _calculateur.CSGDeductible;
    public ResultatAvecTauxEtExplication CRDS => _calculateur.CRDSNonDeductible;
    public ResultatAvecTauxEtExplication FormationProfessionnelle => _calculateur.FormationProfessionnelle;
    public decimal GrandTotal => _calculateur.GrandTotal;
    public decimal AssietteDeCalculDesCotisations { get; private set; }
    public decimal RevenuNet => revenuNet;
    public decimal CotisationsFacultatives => cotisationsFacultatives;
    public int Annee => annee;


    public void Calcule()
    {
        var nombreDIterations = 0;
        var ratioMin = 1m;
        var ratioMax = 1.25m;
        var ratio = ratioMin + (ratioMax - ratioMin) / 2;
        var revenuAPrendreEnCompte = revenuNet + cotisationsFacultatives;

        var assietteDeBase = revenuAPrendreEnCompte * ratio;
        AssietteDeCalculDesCotisations = 0m;
        var diffAssiettes = AssietteDeCalculDesCotisations - assietteDeBase;
        while (Math.Abs(diffAssiettes) > 1)
        {
            _calculateur.CalculeLesCotisations(assietteDeBase);

            AssietteDeCalculDesCotisations = revenuAPrendreEnCompte + _calculateur.CSGNonDeductible.Valeur + _calculateur.CRDSNonDeductible.Valeur;
            diffAssiettes = AssietteDeCalculDesCotisations - assietteDeBase;
            if (AssietteDeCalculDesCotisations <= assietteDeBase)
            {
                ratioMax = ratio;
                ratio -= (ratioMax - ratioMin) / 2;
            }
            else
            {
                ratioMin = ratio;
                ratio += (ratioMax - ratioMin) / 2;
            }

            assietteDeBase = revenuAPrendreEnCompte * ratio;
            nombreDIterations++;
            if (nombreDIterations > NombreDIterationsMaximal)
                throw new InvalidOperationException($"On tente de converger depuis trop longtemps ! Revenu = {revenuNet}, année = {annee}, cotisations facultatives = {cotisationsFacultatives}");
        }
    }
}
