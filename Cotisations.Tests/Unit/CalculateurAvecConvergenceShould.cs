using NFluent;

namespace Cotisations.Tests.Unit;

public class CalculateurAvecConvergenceShould
{
    private readonly VerifySettings _verifySettings = new();

    public CalculateurAvecConvergenceShould()
    {
        _verifySettings.UseDirectory("Snapshots");
    }

    [Test]
    // L'exemple de 40k en 2024 est intéressant car il est :
    // * inférieur à 1 PASS (invalidité/décès non plafonnée)
    // * inférieur à 1.1 PASS (allocations familiales à 0)
    // * entre 0.6 et 1.1 PASS (cotisations maladie, taux progressif entre 4% et 6.7%)
    // * inférieur à 42946 (retraite complémentaire artisans/commerçants : uniquement la tranche à 7%)
    public async Task Calculer_les_cotisations_correctement_pour_un_revenu_de_40k()
    {
        const decimal revenuNet = 40_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        await Verify(convergeur, _verifySettings);
    }

    [Test]
    // Ici, on teste les comportements suivants :
    // * cotisations maladie hors indemnité à 6.5% pour les revenus > 5 PASS
    // * cotisations indemnité maladie à 0% pour les revenus > 5 PASS
    // * allocations à taux fixe car revenu > 1.4 PASS
    // * retraite complémentaire artisans/commerçants plafonnée car revenu > 4 PASS
    public async Task Calculer_les_cotisations_correctement_pour_un_revenu_de_plus_de_5_fois_le_PASS()
    {
        const decimal revenuNet = 300_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        await Verify(convergeur, _verifySettings);
    }

    [Test]
    // Pour tester que les cotises maladie prennent l'assiette en compte et non le revenu net en direct (bug première implem).
    public void Calculer_les_cotisations_maladie_correctement_pour_un_revenu_inferieur_a_40_pct_du_PASS_mais_avec_une_assiette_superieure()
    {
        const decimal revenuNet = 18_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(12m, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(93m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_maladie_correctement_pour_un_revenu_inferieur_a_60_pct_du_PASS_mais_avec_une_assiette_superieure()
    {
        const decimal revenuNet = 27_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(1131m, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(140m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_maladie_correctement_pour_un_revenu_inferieur_a_110_pct_du_PASS_mais_avec_une_assiette_superieure()
    {
        const decimal revenuNet = 51_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(3553, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(265m, 1m);
    }

    // Scénario un peu tiré par les cheveux mais si jamais je rate un autre truc, il faut pouvoir sortir en erreur plutôt que de "converger" éternellement dans une boucle infinie.
    [Test]
    public void Sortir_en_erreur_lorsque_la_convergence_ne_se_fait_pas()
    {
        var convergeur = new CalculateurAvecConvergence(50_000m, 2024, -50_001m);
        Check.ThatCode(convergeur.Calcule).Throws<InvalidOperationException>();
    }
}