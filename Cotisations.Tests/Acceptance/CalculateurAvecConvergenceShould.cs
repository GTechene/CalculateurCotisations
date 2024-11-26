using NFluent;

namespace Cotisations.Tests.Acceptance;

public class CalculateurAvecConvergenceShould
{
    [Test]
    [SetCulture("en-US")]
    public void Calculer_mes_cotisations_2024_correctement_en_convergeant_malgre_la_dependance_circulaire_de_la_CSG_et_CRDS()
    {
        const decimal revenuNet = 62_441m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4349.38m, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(324.58m, 1m);
        Check.That(convergeur.RetraiteDeBase.Valeur).IsCloseTo(8341.61m, 1m);
        Check.That(convergeur.RetraiteComplementaire.Valeur).IsCloseTo(4763.83m, 1m);
        Check.That(convergeur.InvaliditeDeces.Valeur).IsEqualTo(602.784m);
        Check.That(convergeur.AllocationsFamiliales.Valeur).IsCloseTo(2012.4m, 1m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(20394m, 5m);
        Check.That(convergeur.CSGNonDeductible.Valeur).IsCloseTo(2047.46m, 1m);
        Check.That(convergeur.CSGDeductible.Valeur).IsCloseTo(5801.13m, 1m);
        Check.That(convergeur.CRDS.Valeur).IsCloseTo(426.55m, 1m);
        Check.That(convergeur.FormationProfessionnelle.Valeur).IsEqualTo(115.92m);
        Check.That(convergeur.GrandTotal).IsCloseTo(28785m, 5m);

        // Teste que la culture est bien appliquée explicitement dans le code et ne dépend pas de la machine qui le fait tourner.
        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Explication).IsEqualTo("L'assiette de 64 914 € est comprise entre 51 005 € (110% du PASS) et 231 840 € (500% du PASS), donc le taux fixe de 6,7% est appliqué, soit 4 349 € de cotisations.");
    }

    [Test]
    // Ce test est presque raccord (à 99.5%) avec les cotisations réelles demandées par l'URSSAF. C'est normal car le net imposable donné par Numbr (67648) n'est pas bon car basé sur un calcul du simulateur sans filer les cotisations facultatives de 2710.
    public void Calculer_mes_cotisations_2023_à_peu_près_correctement()
    {
        const decimal revenuNet = 65_182m;
        const decimal cotisationsFacultatives = 2710m;

        var convergeur = new CalculateurAvecConvergence(revenuNet, 2023, cotisationsFacultatives);
        convergeur.Calcule();

        // On ne converge pas comme l'URSSAF (on a 70548) mais c'est normal car on ne part pas du même net imposable puisque Numbr a oublié les cotises facultatives.
        Check.That(convergeur.AssietteDeCalculDesCotisations).IsCloseTo(70358m, 191m);

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4468m, 22m);
        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(598m, 2m);
        Check.That(convergeur.RetraiteDeBase.Valeur).IsCloseTo(7967m, 1m);
        Check.That(convergeur.RetraiteComplementaire.Valeur).IsCloseTo(5211m, 26m);
        Check.That(convergeur.InvaliditeDeces.Valeur).IsCloseTo(572m, 1m);
        Check.That(convergeur.AllocationsFamiliales.Valeur).IsCloseTo(2181m, 11m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(21007m, 105m);
        Check.That(convergeur.CSGNonDeductible.Valeur).IsCloseTo(2190m, 10m);
        Check.That(convergeur.CSGDeductible.Valeur).IsCloseTo(6207m, 31m);
        Check.That(convergeur.CRDS.Valeur).IsCloseTo(456m, 2m);
        Check.That(convergeur.FormationProfessionnelle.Valeur).IsEqualTo(109.98m);
        Check.That(convergeur.GrandTotal).IsCloseTo(29881m, 156m);
    }

    [Test]
    // En fait, le simulateur 2023 de l'URSSAF est buggé. Si on répond aux questions pour y mettre Madelin, date de création et statut de l'entreprise (libéral, artisan/commerçant), alors le simulateur prend les taux et plafonds 2024, pas 2023.
    public void Calculer_mes_cotisations_2023_correctement_vis_a_vis_du_simulateur()
    {
        const decimal revenuNet = 65_182m;
        const decimal cotisationsFacultatives = 2710m;

        var convergeur = new CalculateurAvecConvergence(revenuNet, 2023, cotisationsFacultatives);
        convergeur.Calcule();

        // Le simulateur donne 70561 et on a 70451.
        Check.That(convergeur.AssietteDeCalculDesCotisations).IsCloseTo(70561m, 20m);

        // On retrouve le bug ici : le simulateur officiel prend 0.5% pour les cotisations indemnités ; or c'était 0.85% en 2023. Cela donne un total de 5080 pour le simulateur et comme il fait (TotalMaladie - Indemnités) pour calculer les autres cotises, on a bien 5080 - 598 = 4482.
        Check.That(convergeur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4482m, 22m);
        Check.That(convergeur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(598m, 2m);

        // Là encore le simulateur se plante et prend le PASS 2024 (46368) alors qu'on doit prendre le 2023 (43992). J'ai donc 7808 + 159 = 7967 de mon côté là où le simulateur trouve 8230 + 145 = 8375.
        Check.That(convergeur.RetraiteDeBase.Valeur).IsCloseTo(8375m, 408m);

        Check.That(convergeur.RetraiteComplementaire.Valeur).IsCloseTo(5215m, 22m);

        // Idem ici, PASS 2024 au lieu de 2023
        Check.That(convergeur.InvaliditeDeces.Valeur).IsCloseTo(603m, 32m);

        Check.That(convergeur.AllocationsFamiliales.Valeur).IsCloseTo(2187m, 1m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(21007m, 36m);

        // Assiette CSG/CRDS pour le simulateur = 92020. On a 91590.
        Check.That(convergeur.CSGNonDeductible.Valeur).IsCloseTo(2208m, 10m);
        Check.That(convergeur.CSGDeductible.Valeur).IsCloseTo(6257m, 29m);
        Check.That(convergeur.CRDS.Valeur).IsCloseTo(460m, 3m);

        // Là encore un bug
        Check.That(convergeur.FormationProfessionnelle.Valeur).IsCloseTo(134m, 25m);

        // Donc le total s'en trouve forcément affecté.
        Check.That(convergeur.GrandTotal).IsCloseTo(30520m, 495m);
    }

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
        Check.ThatCode(() => convergeur.Calcule()).Throws<InvalidOperationException>();
    }
}