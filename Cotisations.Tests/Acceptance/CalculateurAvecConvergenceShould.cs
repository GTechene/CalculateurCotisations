﻿using NFluent;

namespace Cotisations.Tests.Acceptance;

public class CalculateurAvecConvergenceShould
{
    [Test]
    // L'exemple de 40k en 2024 est intéressant car il est :
    // * inférieur à 1 PASS (invalidité/décès non plafonnée)
    // * inférieur à 1.1 PASS (allocations familiales à 0)
    // * entre 0.6 et 1.1 PASS (cotisations maladie, taux progressif entre 4% et 6.7%)
    // * inférieur à 42946 (retraite complémentaire artisans/commerçants : uniquement la tranche à 7%)
    public void Calculer_les_cotisations_correctement_pour_un_revenu_de_40k()
    {
        const decimal revenuNet = 40_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(2331m, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(208m, 1m);
        Check.That(convergeur.RetraiteDeBase.Valeur).IsCloseTo(7383m, 1m);
        Check.That(convergeur.RetraiteComplementaire.Valeur).IsCloseTo(2912m, 1m);
        Check.That(convergeur.InvaliditeDeces.Valeur).IsCloseTo(541m, 1m);
        Check.That(convergeur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(13373m, 5m);
        Check.That(convergeur.CSGNonDeductible.Valeur).IsCloseTo(1319m, 1m);
        Check.That(convergeur.CSGDeductible.Valeur).IsCloseTo(3738m, 1m);
        Check.That(convergeur.CRDS.Valeur).IsCloseTo(275m, 1m);
        Check.That(convergeur.FormationProfessionnelle.Valeur).IsEqualTo(115.92m);
        Check.That(convergeur.GrandTotal).IsCloseTo(18821m, 5m);
    }

    [Test]
    // Ce test met en exergue une erreur dans la doc des taux (https://www.urssaf.fr/accueil/outils-documentation/taux-baremes/taux-cotisations-ac-plnr.html) :
    // Si le revenu est < 40% du PASS, alors les cotisations pour les indemnités journalières maladie ont un plancher = 40% du PASS. C'est-à-dire = 0.005 * 0.4 * PASS, soit 92.736 € en 2024.
    // C'est visible dans le simulateur (https://mon-entreprise.urssaf.fr/simulateurs/ind%C3%A9pendant).
    public void Calculer_les_cotisations_correctement_pour_un_revenu_inferieur_a_40_pct_du_PASS()
    {
        const decimal revenuNet = 12_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Valeur).IsEqualTo(0m);
        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(92.736m);
        Check.That(convergeur.RetraiteDeBase.Valeur).IsCloseTo(2211m, 1m);
        Check.That(convergeur.RetraiteComplementaire.Valeur).IsCloseTo(872m, 1m);
        Check.That(convergeur.InvaliditeDeces.Valeur).IsCloseTo(162m, 1m);
        Check.That(convergeur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(3338m, 5m);
        Check.That(convergeur.CSGNonDeductible.Valeur).IsCloseTo(379m, 1m);
        Check.That(convergeur.CSGDeductible.Valeur).IsCloseTo(1074m, 1m);
        Check.That(convergeur.CRDS.Valeur).IsCloseTo(78.8m, 1m);
        Check.That(convergeur.FormationProfessionnelle.Valeur).IsEqualTo(115.92m);
        Check.That(convergeur.GrandTotal).IsCloseTo(4986m, 5m);
    }

    [Test]
    // Ici, on teste les comportements suivants :
    // * cotisations maladie hors indemnité à 6.5% pour les revenus > 5 PASS
    // * cotisations indemnité maladie à 0% pour les revenus > 5 PASS
    // * allocations à taux fixe car revenu > 1.4 PASS
    // * retraite complémentaire artisans/commerçants plafonnée car revenu > 4 PASS
    public void Calculer_les_cotisations_correctement_pour_un_revenu_de_plus_de_5_fois_le_PASS()
    {
        const decimal revenuNet = 300_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(20655m, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(1159m, 1m);
        Check.That(convergeur.RetraiteDeBase.Valeur).IsCloseTo(9816m, 1m);
        Check.That(convergeur.RetraiteComplementaire.Valeur).IsEqualTo(14408.3m);
        Check.That(convergeur.InvaliditeDeces.Valeur).IsEqualTo(602.784m);
        Check.That(convergeur.AllocationsFamiliales.Valeur).IsCloseTo(9630m, 1m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(56271m, 5m);
        Check.That(convergeur.CSGNonDeductible.Valeur).IsCloseTo(8806m, 1m);
        Check.That(convergeur.CSGDeductible.Valeur).IsCloseTo(24950m, 1m);
        Check.That(convergeur.CRDS.Valeur).IsCloseTo(1834.6m, 1m);
        Check.That(convergeur.FormationProfessionnelle.Valeur).IsEqualTo(115.92m);
        Check.That(convergeur.GrandTotal).IsCloseTo(91977m, 5m);
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

    [Test]
    public void Calculer_les_cotisations_minimales_correctement_pour_un_revenu_inferieur_a_4000_en_2024()
    {
        const decimal revenuNet = 1_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet, 2024);
        convergeur.Calcule();

        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(93m, 1m);
        Check.That(convergeur.InvaliditeDeces.Valeur).IsCloseTo(69m, 1m);
        Check.That(convergeur.RetraiteDeBase.Valeur).IsCloseTo(931m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_minimales_correctement_pour_un_revenu_inferieur_a_4000_en_2023()
    {
        const decimal revenuNet = 1_000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet, 2023);
        convergeur.Calcule();

        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(88m, 1m);
        Check.That(convergeur.InvaliditeDeces.Valeur).IsCloseTo(66m, 1m);
        Check.That(convergeur.RetraiteDeBase.Valeur).IsCloseTo(898m, 1m);
    }
}