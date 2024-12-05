namespace Cotisations;

public class Calculateur2025 : ICalculateur
{
    private const decimal TauxPlancherDeLAbattement = 0.0176m;
    private const decimal TauxPlafondDeLAbattement = 1.30m;
    private const decimal TauxAbattement = 0.26m;

    private readonly PlafondAnnuelSecuriteSociale PASS;

    public Calculateur2025()
    {
        PASS = new PlafondAnnuelSecuriteSociale(2025);
    }

    public void CalculeLesCotisations(decimal revenu)
    {
        var abattementPlancher = TauxPlancherDeLAbattement * PASS.Valeur;
        var abattementPlafond = TauxPlafondDeLAbattement * PASS.Valeur;
        var abattement = Math.Min(Math.Max(revenu * TauxAbattement, abattementPlancher),  abattementPlafond);

        Assiette = revenu - abattement;

        CalculeLesCotisationsMaladieHorsIndemnites(revenu);
    }

    private void CalculeLesCotisationsMaladieHorsIndemnites(decimal revenu)
    {
        if (revenu <= PASS.Valeur * 0.2m)
        {
            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxEtExplication(0m, 0m, $"Le montant de {revenu:C0} est inférieur à {PASS.Valeur40Pct:C0} (20% du PASS). Il n'y a donc pas de cotisation maladie à payer.");
        }
        else if (revenu > PASS.Valeur * 0.2m && revenu <= PASS.Valeur40Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur * 0.2m, PASS.Valeur40Pct, 0m, 0.015m);
            var valeur = revenu * tauxApplicable;
            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxEtExplication(valeur, tauxApplicable, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur * 0.2m:C0} (20% du PASS) et {PASS.Valeur40Pct:C0} (40% du PASS), donc un taux progressif entre 0% et {0.015m * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.");
        }
        else if (revenu > PASS.Valeur40Pct && revenu <= PASS.Valeur60Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur40Pct, PASS.Valeur60Pct, 0.015m, 0.04m);
            var valeur = revenu * tauxApplicable;
            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxEtExplication(valeur, tauxApplicable, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur60Pct:C0} (60% du PASS), donc un taux progressif entre {0.015m * 100:F1}% et {0.04m * 100:N0}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.");
        }
        else if (revenu > PASS.Valeur60Pct && revenu <= PASS.Valeur110Pct)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur60Pct, PASS.Valeur110Pct, 0.04m, 0.065m);
            var valeur = revenu * tauxApplicable;
            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxEtExplication(valeur, tauxApplicable, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur60Pct:C0} (60% du PASS) et {PASS.Valeur110Pct:C0} (110% du PASS), donc un taux progressif entre {0.04m * 100:N0}% et {0.065m * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.");
        }
        else if (revenu > PASS.Valeur110Pct && revenu <= PASS.Valeur * 2)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur110Pct, PASS.Valeur * 2, 0.065m, 0.077m);
            var valeur = revenu * tauxApplicable;
            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxEtExplication(valeur, tauxApplicable, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur110Pct:C0} (110% du PASS) et {PASS.Valeur * 2:C0} (2 PASS), donc un taux progressif entre {0.065m * 100:F1}% et {0.077m * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.");
        }
        else if (revenu > PASS.Valeur * 2 && revenu <= PASS.Valeur * 3)
        {
            var tauxApplicable = CalculeLeTauxProgressif(revenu, PASS.Valeur * 2, PASS.Valeur * 3, 0.077m, 0.085m);
            var valeur = revenu * tauxApplicable;
            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxEtExplication(valeur, tauxApplicable, $"Le montant de {revenu:C0} est compris entre {PASS.Valeur * 2:C0} (2 PASS) et {PASS.Valeur * 3:C0} (3 PASS), donc un taux progressif entre {0.077m * 100:F1}% et {0.085m * 100:F1}% est appliqué. Ici il s'agit de {tauxApplicable * 100:F1}%, soit {valeur:C0} de cotisations.");
        }
        else if (revenu > PASS.Valeur * 3)
        {
            var partTauxMax = PASS.Valeur * 3;
            var partTauxReduit = revenu - partTauxMax;
            var valeur = partTauxMax * 0.085m + partTauxReduit * 0.065m;
            // TODO: trouver un type de retour qui va bien...
            MaladieHorsIndemnitesJournalieres = new ResultatAvecTauxEtExplication(valeur, 0.085m, $"Le montant de {revenu:C0} est supérieur à {PASS.Valeur * 3:C0} (3 PASS), donc un taux de {0.085m * 100:F1}% est appliqué à la part des revenus dans la limite de 3 PASS et le taux fixe de {0.065m * 100:F1}% est appliqué à la part des revenus qui y est supérieure, soit {valeur:C0} de cotisations.");
        }
    }

    private decimal CalculeLeTauxProgressif(decimal assiette, decimal valeurPlancher, decimal valeurPlafond, decimal tauxPlancher, decimal tauxPlafond)
    {
        var diffDeTaux = tauxPlafond - tauxPlancher;
        var ratio = (assiette - valeurPlancher) / (valeurPlafond - valeurPlancher);
        var tauxApplicable = diffDeTaux * ratio + tauxPlancher;

        return tauxApplicable;
    }

    public ResultatAvecTauxEtExplication MaladieHorsIndemnitesJournalieres { get; private set; }
    public ResultatAvecTauxEtExplication MaladieIndemnitesJournalieres { get; }
    public ResultatAvecTauxMultiplesEtExplication RetraiteDeBase { get; }
    public ResultatAvecTauxMultiplesEtExplication RetraiteComplementaire { get; }
    public ResultatAvecTauxEtExplication InvaliditeDeces { get; }
    public ResultatAvecTauxEtExplication AllocationsFamiliales { get; }
    public ResultatAvecTauxEtExplication CSGNonDeductible { get; }
    public ResultatAvecTauxEtExplication CSGDeductible { get; }
    public ResultatAvecTauxEtExplication CRDSNonDeductible { get; }
    public ResultatAvecTauxEtExplication FormationProfessionnelle { get; }
    public decimal Assiette { get; private set; }
}