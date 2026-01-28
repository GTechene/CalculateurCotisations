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
    public decimal Abattement => annee >= 2025 ? ((ICalculateurPostReforme2024) Calculateur).Abattement : 0m;
    public decimal RevenuBrut { get; private set; }

    public void Calcule_Avant_2025()
    {
        var nombreDIterations = 0;
        var ratioMin = 1m;
        var ratioMax = 1.25m;
        var ratio = ratioMin + (ratioMax - ratioMin) / 2;
        var revenuAPrendreEnCompte = revenuNet + cotisationsFacultatives;

        var assietteDeBase = revenuAPrendreEnCompte * ratio;
        AssietteDeCalculDesCotisations = 0m;
        while (nombreDIterations <= NombreDIterationsMaximal)
        {
            Calculateur.CalculeLesCotisations(assietteDeBase);

            AssietteDeCalculDesCotisations = revenuAPrendreEnCompte + Calculateur.CSGNonDeductible.Valeur + Calculateur.CRDSNonDeductible.Valeur;
            var diffAssiettes = AssietteDeCalculDesCotisations - assietteDeBase;
            if (Math.Abs(diffAssiettes) <= 1)
            {
                AssietteDeCalculDesCotisations = assietteDeBase;
                break;
            }

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
        }

        if(nombreDIterations > NombreDIterationsMaximal)
            throw new InvalidOperationException($"On tente de converger depuis trop longtemps ! Revenu = {revenuNet}, année = {annee}, cotisations facultatives = {cotisationsFacultatives}");
    }

    // TODO : à terme, une fois les API officielle dispo, on les sollicitera pour récupérer le revenu brut et éviter de faire la convergence nous-mêmes.
    public void Calcule_Depuis_2025()
    {
        var nombreDIterations = 0;
        var ratioMin = 1.2m;
        var ratioMax = 3m;
        var ratio = 1.4m;
        // TODO : quid des cotisations Madelin depuis 2025 ? A priori on fait comme avant mais on attend de voir.
        var revenuAPrendreEnCompte = revenuNet + cotisationsFacultatives;

        RevenuBrut = revenuAPrendreEnCompte * ratio;
        while (nombreDIterations <= NombreDIterationsMaximal)
        {
            Calculateur.CalculeLesCotisations(RevenuBrut);

            var nouveauRevenuNet = RevenuBrut - Calculateur.GrandTotal;
            var diffNet = revenuAPrendreEnCompte - nouveauRevenuNet;
            if (Math.Abs(diffNet) <= 1)
            {
                AssietteDeCalculDesCotisations = Calculateur.AssietteDeCalculDesCotisations;
                break;
            }

            if (revenuAPrendreEnCompte <= nouveauRevenuNet)
            {
                ratioMax = ratio;
                ratio -= (ratioMax - ratioMin) / 2;
            }
            else
            {
                ratioMin = ratio;
                ratio += (ratioMax - ratioMin) / 2;
            }

            RevenuBrut = revenuAPrendreEnCompte * ratio;
            nombreDIterations++;
        }

        if (nombreDIterations > NombreDIterationsMaximal)
            throw new InvalidOperationException($"On tente de converger depuis trop longtemps ! Revenu = {revenuNet}, année = {annee}, cotisations facultatives = {cotisationsFacultatives}");
    }
}
