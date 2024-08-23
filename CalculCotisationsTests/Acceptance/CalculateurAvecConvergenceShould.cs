﻿using CalculCotisations;
using NFluent;
using NUnit.Framework;

namespace CalculCotisationsTests.Acceptance;

public class CalculateurAvecConvergenceShould
{
    [Test]
    public void Calculer_mes_cotisations_2024_correctement_en_convergeant_malgre_la_dependance_circulaire_de_la_CSG_et_CRDS()
    {
        const decimal revenuNet = 62441m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres).IsCloseTo(4349.38m, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres).IsCloseTo(324.58m, 1m);
        Check.That(convergeur.RetraiteDeBase).IsCloseTo(8341.61m, 1m);
        Check.That(convergeur.RetraiteComplementaire).IsCloseTo(4763.83m, 1m);
        Check.That(convergeur.InvaliditeDeces).IsCloseTo(602.78m, 1m);
        Check.That(convergeur.AllocationsFamiliales).IsCloseTo(2012.4m, 1m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(20394m, 5m);
        Check.That(convergeur.CSGNonDeductible).IsCloseTo(2047.46m, 1m);
        Check.That(convergeur.CSGDeductible).IsCloseTo(5801.13m, 1m);
        Check.That(convergeur.CRDS).IsCloseTo(426.55m, 1m);
        Check.That(convergeur.FormationProfessionnelle).IsEqualTo(115.92m);
        Check.That(convergeur.GrandTotal).IsCloseTo(28785m, 5m);
    }

    [Test]
    public void Calculer_mes_cotisations_2023_correctement()
    {
        const decimal revenuNet = 65182m;

        var convergeur = new CalculateurAvecConvergence(revenuNet, 2023);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres).IsCloseTo(4538m, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres).IsCloseTo(338.71m, 1m);
        Check.That(convergeur.RetraiteDeBase).IsCloseTo(7951m, 1m);
        Check.That(convergeur.RetraiteComplementaire).IsCloseTo(4990m, 1m);
        Check.That(convergeur.InvaliditeDeces).IsCloseTo(571.9m, 1m);
        Check.That(convergeur.AllocationsFamiliales).IsCloseTo(2100m, 1m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(20490m, 5m);
        Check.That(convergeur.CSGNonDeductible).IsCloseTo(2118m, 1m);
        Check.That(convergeur.CSGDeductible).IsCloseTo(6000m, 1m);
        Check.That(convergeur.CRDS).IsCloseTo(441.22m, 1m);
        Check.That(convergeur.FormationProfessionnelle).IsEqualTo(109.98m);
        Check.That(convergeur.GrandTotal).IsCloseTo(29158m, 5m);
    }

    [Test]
    public void Calculer_les_cotisations_correctement_pour_un_revenu_de_40k()
    {
        const decimal revenuNet = 40000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres).IsCloseTo(2331m, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres).IsCloseTo(208m, 1m);
        Check.That(convergeur.RetraiteDeBase).IsCloseTo(7383m, 1m);
        Check.That(convergeur.RetraiteComplementaire).IsCloseTo(2912m, 1m);
        Check.That(convergeur.InvaliditeDeces).IsCloseTo(541m, 1m);
        Check.That(convergeur.AllocationsFamiliales).IsEqualTo(0m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(13373m, 5m);
        Check.That(convergeur.CSGNonDeductible).IsCloseTo(1319m, 1m);
        Check.That(convergeur.CSGDeductible).IsCloseTo(3738m, 1m);
        Check.That(convergeur.CRDS).IsCloseTo(275m, 1m);
        Check.That(convergeur.FormationProfessionnelle).IsEqualTo(115.92m);
        Check.That(convergeur.GrandTotal).IsCloseTo(18821m, 5m);
    }

    [Test]
    // Ce test met en exergue une erreur dans la doc des taux (https://www.urssaf.fr/accueil/outils-documentation/taux-baremes/taux-cotisations-ac-plnr.html) :
    // Si le revenu est < 40% du PASS, alors les cotisations pour les indemnités journalières maladie ont un plancher = 40% du PASS.
    // C'est visible dans le simulateur (https://mon-entreprise.urssaf.fr/simulateurs/ind%C3%A9pendant).
    public void Calculer_les_cotisations_correctement_pour_un_revenu_de_12k()
    {
        const decimal revenuNet = 12000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres).IsEqualTo(0m);
        Check.That(convergeur.MaladieIndemnitesJournalieres).IsCloseTo(93m, 1m);
        Check.That(convergeur.RetraiteDeBase).IsCloseTo(2211m, 1m);
        Check.That(convergeur.RetraiteComplementaire).IsCloseTo(872m, 1m);
        Check.That(convergeur.InvaliditeDeces).IsCloseTo(162m, 1m);
        Check.That(convergeur.AllocationsFamiliales).IsEqualTo(0m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(3338m, 5m);
        Check.That(convergeur.CSGNonDeductible).IsCloseTo(379m, 1m);
        Check.That(convergeur.CSGDeductible).IsCloseTo(1074m, 1m);
        Check.That(convergeur.CRDS).IsCloseTo(78.8m, 1m);
        Check.That(convergeur.FormationProfessionnelle).IsEqualTo(115.92m);
        Check.That(convergeur.GrandTotal).IsCloseTo(4986m, 5m);
    }

    [Test]
    public void Calculer_les_cotisations_correctement_pour_un_revenu_de_plus_de_5_fois_le_PASS()
    {
        const decimal revenuNet = 300000m;

        var convergeur = new CalculateurAvecConvergence(revenuNet);
        convergeur.Calcule();

        Check.That(convergeur.MaladieHorsIndemnitesJournalieres).IsCloseTo(20655m, 1m);
        Check.That(convergeur.MaladieIndemnitesJournalieres).IsCloseTo(1159m, 1m);
        Check.That(convergeur.RetraiteDeBase).IsCloseTo(9816m, 1m);
        Check.That(convergeur.RetraiteComplementaire).IsCloseTo(14408m, 1m);
        Check.That(convergeur.InvaliditeDeces).IsCloseTo(603m, 1m);
        Check.That(convergeur.AllocationsFamiliales).IsCloseTo(9630m, 1m);
        Check.That(convergeur.TotalCotisationsObligatoires).IsCloseTo(56271m, 5m);
        Check.That(convergeur.CSGNonDeductible).IsCloseTo(8806m, 1m);
        Check.That(convergeur.CSGDeductible).IsCloseTo(24950m, 1m);
        Check.That(convergeur.CRDS).IsCloseTo(1834.6m, 1m);
        Check.That(convergeur.FormationProfessionnelle).IsEqualTo(115.92m);
        Check.That(convergeur.GrandTotal).IsCloseTo(91977m, 5m);
    }
}