using NFluent;

namespace Cotisations.Tests.Unit;

public class Calculateur2023Should
{
    [Test]
    // Ce test nous permet de vérifier que j'ai bien corrigé un bug sur le calcul des cotisations maladie pour un revenu entre 0.4 et 0.6 PASS (taux progressif à partir de 3.65% et pas 4% comme en 2024)
    public void Calculer_les_cotisations_maladie_correctement_pour_un_revenu_de_25k_en_2023()
    {
        const decimal revenuNet = 25_000m;

        var calculateur = new Calculateur2023();
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(767.8m, 1m);
    }

    [Test]
    // Ce test met en exergue une erreur dans la doc des taux (https://www.urssaf.fr/accueil/outils-documentation/taux-baremes/taux-cotisations-ac-plnr.html) :
    // Si le revenu est < 40% du PASS, alors les cotisations pour les indemnités journalières maladie ont un plancher = 40% du PASS. C'est-à-dire = 0.005 * 0.4 * PASS, soit 87.984 € en 2023.
    // C'est visible dans le simulateur (https://mon-entreprise.urssaf.fr/simulateurs/ind%C3%A9pendant).
    public void Calculer_les_cotisations_correctement_pour_un_revenu_inferieur_a_40_pct_du_PASS()
    {
        const decimal revenuNet = 12_000m;

        var calculateur = new Calculateur2023();
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(87.984m);
    }

    [Test]
    public void Calculer_les_cotisations_minimales_correctement_pour_un_revenu_inferieur_a_4000()
    {
        const decimal revenuNet = 1_000m;

        var calculateur = new Calculateur2023();
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(88m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(66m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(898m, 1m);
    }
}
