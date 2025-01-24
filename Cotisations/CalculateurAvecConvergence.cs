namespace Cotisations;

/// <summary>
/// Calcule les cotisations en faisant converger l'assiette de base (revenu net + CSG non déductible + CRDS) avec l'assiette calculée. En effet, pour calculer CSG et CRDS, il faut connaître le total des cotisations obligatoires. Or celles-ci ne sont calculables qu'en connaissant l'assiette de base... qui dépend de la CSG et de la CRDS.
/// J'ai donc opté pour un ratio "au doigt mouillé" pour le premier calcul (1.125) puis je fais converger par dichotomie en fonction de l'assiette calculée à partir de ce premier calcul.
/// </summary>

// TODO: limiter l'année à 2024 maximum
public class CalculateurAvecConvergence(decimal revenuNet, int annee = 2024, decimal cotisationsFacultatives = 0m)
{
    public ICalculateur Calculateur { get; } = Calculateurs.TrouveUnCalculateur(annee);

    private const int NombreDIterationsMaximal = 100;

    public decimal TotalCotisationsObligatoires => Calculateur.TotalCotisationsObligatoires;
    public ResultatAvecExplicationEtTaux MaladieHorsIndemnitesJournalieres => Calculateur.MaladieHorsIndemnitesJournalieres;
    public ResultatAvecTauxUniqueEtExplication MaladieIndemnitesJournalieres => Calculateur.MaladieIndemnitesJournalieres;
    public ResultatAvecTauxMultiplesEtExplication RetraiteDeBase => Calculateur.RetraiteDeBase;
    public ResultatAvecTauxMultiplesEtExplication RetraiteComplementaire => Calculateur.RetraiteComplementaire;
    public ResultatAvecTauxUniqueEtExplication InvaliditeDeces => Calculateur.InvaliditeDeces;
    public ResultatAvecTauxUniqueEtExplication AllocationsFamiliales => Calculateur.AllocationsFamiliales;
    public ResultatAvecTauxUniqueEtExplication CSGNonDeductible => Calculateur.CSGNonDeductible;
    public ResultatAvecTauxUniqueEtExplication CSGDeductible => Calculateur.CSGDeductible;
    public ResultatAvecTauxUniqueEtExplication CRDS => Calculateur.CRDSNonDeductible;
    public ResultatAvecTauxUniqueEtExplication FormationProfessionnelle => Calculateur.FormationProfessionnelle;
    public decimal GrandTotal => Calculateur.GrandTotal;
    public decimal AssietteDeCalculDesCotisations { get; private set; }

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
            Calculateur.CalculeLesCotisations(assietteDeBase);

            AssietteDeCalculDesCotisations = revenuAPrendreEnCompte + Calculateur.CSGNonDeductible.Valeur + Calculateur.CRDSNonDeductible.Valeur;
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
