using CalculCotisations;
using NFluent;
using NUnit.Framework;
// On désactive les warnings concernant l'année car il faut que les tests soient pérennes en 2025 et après :)
// ReSharper disable RedundantArgumentDefaultValue

namespace CalculCotisationsTests.Unit;

public class CalculateurSimpleShould
{
    [Test]
    public void Renvoyer_0_euro_de_cotises_maladie_avec_revenu_inferieur_a_40_pct_du_PASS()
    {
        const decimal revenuNet = 18000m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        const decimal cotisationsAttendues = 0m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres).IsEqualTo(cotisationsAttendues);

        // 0.5% appliqués au plancher de 40% du PASS ; pas dans la doc officielle mais découvert dans le simulateur...
        const decimal cotisationsAttenduesPourLesIndemnites = 92.736m;
        Check.That(calculateur.MaladieIndemnitesJournalieres).IsEqualTo(cotisationsAttenduesPourLesIndemnites);
    }

    [Test]
    public void Calculer_cotises_maladie_avec_revenu_entre_40_pct_et_60_pct_du_PASS()
    {
        const decimal revenuNet = 25000m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        // Taux progressif (~2.78% ici)
        const decimal expectedCotisations = 695.8m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres).IsCloseTo(expectedCotisations, 1m);
    }

    [Test]
    public void Calculer_cotises_maladie_avec_revenu_entre_60_pct_et_110_pct_du_PASS()
    {
        const decimal revenuNet = 40000m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        // Taux progressif (~5.42% ici)
        const decimal expectedCotisations = 2167m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres).IsCloseTo(expectedCotisations, 1m);
    }

    [Test]
    public void Calculer_cotises_maladie_avec_revenu_entre_110_pct_du_PASS_et_500_pct_du_PASS()
    {
        const int revenuNet = 60000;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        const decimal expectedCotisations = revenuNet * 0.067m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres).IsEqualTo(expectedCotisations);
        const decimal expectedIndemnites = revenuNet * 0.005m;
        Check.That(calculateur.MaladieIndemnitesJournalieres).IsEqualTo(expectedIndemnites);
    }

    [Test]
    public void Calculer_les_cotisations_maladie_avec_un_revenu_superieur_a_5_PASS()
    {
        const int revenuNet = 300000;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        var indemnitesAttendues = PlafondAnnuelSecuriteSociale.PASS.Valeur500Pct * 0.005m;
        Check.That(calculateur.MaladieIndemnitesJournalieres).IsEqualTo(indemnitesAttendues);

        // Le calcul est : 6.7% sur les revenus qui valent 5 fois le PASS + 6.5% pour les revenus au-dessus. Donc ici : 0.067 * PASS * 5 + 0.065 * (300000 - PASS * 5).
        const decimal cotisationsAttendues = 19963.68m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres).IsEqualTo(cotisationsAttendues);
    }

    [Test]
    public void Calculer_la_retraite_de_base_avec_un_revenu_depassant_le_PASS()
    {
        const int revenuNet = 64913;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        // 17.75% pour les revenus <= 1 PASS + 0.6% pour les revenus > 1 PASS
        Check.That(calculateur.RetraiteDeBase).IsEqualTo(8341.59m);
    }

    [Test]
    public void Calculer_la_retraite_complementaire_des_artisans_avec_un_revenu_inferieur_a_42946_euros()
    {
        const decimal revenuNet = 40000m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        Check.That(calculateur.RetraiteComplementaire).IsEqualTo(2800m);
    }

    [Test]
    public void Calculer_la_retraite_complementaire_des_artisans_avec_un_revenu_depassant_42946_euros_mais_inferieur_a_4_PASS()
    {
        const decimal revenuNet = 64913m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        // 7% pour les revenus <= 42946 + 8% pour les revenus > 42946
        Check.That(calculateur.RetraiteComplementaire).IsEqualTo(4763.58m);
    }

    [Test]
    public void Calculer_la_retraite_complementaire_des_artisans_avec_un_revenu_superieur_a_4_PASS()
    {
        const decimal revenuNet = 190000m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        // 7% pour les revenus <= 42946 + 8% pour les revenus > 42946 ET < 4 PASS ; 0% ensuite
        Check.That(calculateur.RetraiteComplementaire).IsEqualTo(14408.3m);
    }

    [Test]
    public void Calculer_l_invalidite_deces_pour_un_revenu_inferieur_a_1_PASS()
    {
        const decimal revenuNet = 40000m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        // 1.3% des revenus
        const decimal cotisationsAttendues = 520m;
        Check.That(calculateur.InvaliditeDeces).IsEqualTo(cotisationsAttendues);
    }

    [Test]
    public void Calculer_l_invalidite_deces_pour_un_revenu_superieur_a_1_PASS()
    {
        const decimal revenuNet = 64913m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        // 1.3% des revenus <= 1 PASS, 0% ensuite.
        const decimal cotisationsAttendues = 602.784m;
        Check.That(calculateur.InvaliditeDeces).IsEqualTo(cotisationsAttendues);
    }

    [Test]
    public void Renvoyer_0_euro_pour_les_cotisations_des_allocations_familiales_avec_un_revenu_inferieur_a_110_pct_du_PASS()
    {
        const decimal revenuNet = 50000m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        Check.That(calculateur.AllocationsFamiliales).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_allocations_familiales_avec_un_revenu_entre_110_pct_et_140_pct_du_PASS()
    {
        const decimal revenuNet = 60000m;
        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        // Taux progressif (~2% ici)
        const decimal cotisationsAttendues = 1202.77m;
        Check.That(calculateur.AllocationsFamiliales).IsCloseTo(cotisationsAttendues, 1m);
    }

    [Test]
    public void Calculer_les_allocations_familiales_avec_un_revenu_depassant_140_pct_du_PASS()
    {
        const decimal revenuNet = 70000m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        Check.That(calculateur.AllocationsFamiliales).IsEqualTo(0.031m * revenuNet);
    }

    [Test]
    public void Calculer_la_CSG_et_la_CRDS()
    {
        const decimal revenuNet = 70000m;

        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        var revenuPrisEnComptePourCSGEtCRDS = revenuNet + calculateur.TotalCotisationsObligatoires;

        Check.That(calculateur.CSGNonDeductible).IsEqualTo(0.024m * revenuPrisEnComptePourCSGEtCRDS);
        Check.That(calculateur.CSGDeductible).IsEqualTo(0.068m * revenuPrisEnComptePourCSGEtCRDS);
        Check.That(calculateur.CRDSNonDeductible).IsEqualTo(0.005m * revenuPrisEnComptePourCSGEtCRDS);
    }

    [Test]
    public void Calculer_la_formation_professionnelle()
    {
        var revenuNet = new Random().Next(10000, 300000);
        var calculateur = new CalculateurSimple(revenuNet, 2024);
        calculateur.CalculeLesCotisations();

        // 0.25% du PASS quoi qu'il arrive
        const decimal cotisationsAttendues = 115.92m;
        Check.That(calculateur.FormationProfessionnelle).IsEqualTo(cotisationsAttendues);
    }
}