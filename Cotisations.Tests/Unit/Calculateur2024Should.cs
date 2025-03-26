using NFluent;

namespace Cotisations.Tests.Unit;

public class Calculateur2024Should
{
    [Test]
    // Ce test met en exergue une erreur dans la doc des taux (https://www.urssaf.fr/accueil/outils-documentation/taux-baremes/taux-cotisations-ac-plnr.html) :
    // Si le revenu est < 40% du PASS, alors les cotisations pour les indemnités journalières maladie ont un plancher = 40% du PASS. C'est-à-dire = 0.005 * 0.4 * PASS, soit 92.736 € en 2024.
    // C'est visible dans le simulateur (https://mon-entreprise.urssaf.fr/simulateurs/ind%C3%A9pendant).
    public void Calculer_les_cotisations_correctement_pour_un_revenu_inferieur_a_40_pct_du_PASS()
    {
        const decimal revenuNet = 12_000m;

        var calculateur = new Calculateur2024();
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(92.736m);
    }

    [Test]
    public void Calculer_les_cotisations_minimales_correctement_pour_un_revenu_inferieur_a_4000()
    {
        const decimal revenuNet = 1_000m;

        var calculateur = new Calculateur2024();
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(93m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(69m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(931m, 1m);
    }
}
