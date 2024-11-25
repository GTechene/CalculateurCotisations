using NFluent;

// On désactive les warnings concernant l'année car il faut que les tests soient pérennes en 2025 et après :)
// ReSharper disable RedundantArgumentDefaultValue

namespace Cotisations.Tests.Unit;

public class CalculateurSimpleShould
{
    [Test]
    public void Renvoyer_0_euro_de_cotises_maladie_avec_revenu_inferieur_a_40_pct_du_PASS()
    {
        const decimal revenuNet = 17000m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        const decimal cotisationsAttendues = 0m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsEqualTo(cotisationsAttendues);

        // 0.5% appliqués au plancher de 40% du PASS ; pas dans la doc officielle mais découvert dans le simulateur...
        const decimal cotisationsAttenduesPourLesIndemnites = 92.736m;
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(cotisationsAttenduesPourLesIndemnites);
    }

    [Test]
    public void Calculer_cotises_maladie_avec_revenu_entre_40_pct_et_60_pct_du_PASS()
    {
        const decimal revenuNet = 25000m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        // Taux progressif (~2.78% ici)
        const decimal expectedCotisations = 695.8m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(expectedCotisations, 1m);
    }

    [Test]
    public void Calculer_cotises_maladie_avec_revenu_entre_60_pct_et_110_pct_du_PASS()
    {
        const decimal revenuNet = 40000m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        // Taux progressif (~5.42% ici)
        const decimal expectedCotisations = 2167m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(expectedCotisations, 1m);
    }

    [Test]
    public void Calculer_cotises_maladie_avec_revenu_entre_110_pct_du_PASS_et_500_pct_du_PASS()
    {
        const int revenuNet = 60000;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        const decimal expectedCotisations = revenuNet * 0.067m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsEqualTo(expectedCotisations);
        const decimal expectedIndemnites = revenuNet * 0.005m;
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(expectedIndemnites);
    }

    [Test]
    public void Calculer_les_cotisations_maladie_avec_un_revenu_superieur_a_5_PASS()
    {
        const int revenuNet = 300000;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        var pass = new PlafondAnnuelSecuriteSociale(2024);
        var indemnitesAttendues = pass.Valeur500Pct * 0.005m;
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(indemnitesAttendues);

        // Le calcul est : 6.7% sur les revenus qui valent 5 fois le PASS + 6.5% pour les revenus au-dessus. Donc ici : 0.067 * PASS * 5 + 0.065 * (300000 - PASS * 5).
        const decimal cotisationsAttendues = 19963.68m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsEqualTo(cotisationsAttendues);
    }

    [Test]
    public void Calculer_la_retraite_de_base_avec_un_revenu_depassant_le_PASS()
    {
        const int revenuNet = 64913;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        // 17.75% pour les revenus <= 1 PASS + 0.6% pour les revenus > 1 PASS
        Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(8341.59m);
    }

    [Test]
    public void Calculer_la_retraite_complementaire_des_artisans_avec_un_revenu_inferieur_a_42946_euros()
    {
        const decimal revenuNet = 40000m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(2800m);
    }

    [Test]
    public void Calculer_la_retraite_complementaire_des_artisans_avec_un_revenu_depassant_42946_euros_mais_inferieur_a_4_PASS()
    {
        const decimal revenuNet = 64913m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        // 7% pour les revenus <= 42946 + 8% pour les revenus > 42946
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4763.58m);
    }

    [Test]
    public void Calculer_la_retraite_complementaire_des_artisans_avec_un_revenu_superieur_a_4_PASS()
    {
        const decimal revenuNet = 190000m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        // 7% pour les revenus <= 42946 + 8% pour les revenus > 42946 ET < 4 PASS ; 0% ensuite
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(14408.3m);
    }

    [Test]
    public void Calculer_l_invalidite_deces_pour_un_revenu_inferieur_a_1_PASS()
    {
        const decimal revenuNet = 40000m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        // 1.3% des revenus
        const decimal cotisationsAttendues = 520m;
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(cotisationsAttendues);
    }

    [Test]
    public void Calculer_l_invalidite_deces_pour_un_revenu_superieur_a_1_PASS()
    {
        const decimal revenuNet = 64913m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        // 1.3% des revenus <= 1 PASS, 0% ensuite.
        const decimal cotisationsAttendues = 602.784m;
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(cotisationsAttendues);
    }

    [Test]
    public void Renvoyer_0_euro_pour_les_cotisations_des_allocations_familiales_avec_un_revenu_inferieur_a_110_pct_du_PASS()
    {
        const decimal revenuNet = 50000m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_allocations_familiales_avec_un_revenu_entre_110_pct_et_140_pct_du_PASS()
    {
        const decimal revenuNet = 60000m;
        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        // Taux progressif (~2% ici)
        const decimal cotisationsAttendues = 1202.77m;
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(cotisationsAttendues, 1m);
    }

    [Test]
    public void Calculer_les_allocations_familiales_avec_un_revenu_depassant_140_pct_du_PASS()
    {
        const decimal revenuNet = 70000m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0.031m * revenuNet);
    }

    [Test]
    public void Calculer_la_CSG_et_la_CRDS()
    {
        const decimal revenuNet = 70000m;

        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        var revenuPrisEnComptePourCSGEtCRDS = revenuNet + calculateur.TotalCotisationsObligatoires;

        Check.That(calculateur.CSGNonDeductible.Valeur).IsEqualTo(0.024m * revenuPrisEnComptePourCSGEtCRDS);
        Check.That(calculateur.CSGDeductible.Valeur).IsEqualTo(0.068m * revenuPrisEnComptePourCSGEtCRDS);
        Check.That(calculateur.CRDSNonDeductible.Valeur).IsEqualTo(0.005m * revenuPrisEnComptePourCSGEtCRDS);
    }

    [Test]
    public void Calculer_la_formation_professionnelle()
    {
        var revenuNet = new Random().Next(10000, 300000);
        var calculateur = Calculateurs.TrouveUnCalculateur(2024);
        calculateur.CalculeLesCotisations(revenuNet);

        // 0.25% du PASS quoi qu'il arrive
        const decimal cotisationsAttendues = 115.92m;
        Check.That(calculateur.FormationProfessionnelle.Valeur).IsEqualTo(cotisationsAttendues);
    }

    [Test]
    public void Calculer_les_cotisations_pour_2023_pour_un_revenu_superieur_a_110_pct_du_PASS()
    {
        const decimal revenuNet = 70358m;
        var calculateur = Calculateurs.TrouveUnCalculateur(2023);
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(572m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4468m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(598m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_pour_2023_pour_un_revenu_inferieur_a_40_pct_du_PASS()
    {
        const decimal revenuNet = 15000m;
        var calculateur = Calculateurs.TrouveUnCalculateur(2023);
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(195m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsEqualTo(0m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(88m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_pour_2023_pour_un_revenu_superieur_a_500_pct_du_PASS()
    {
        const decimal revenuNet = 250000m;
        var calculateur = Calculateurs.TrouveUnCalculateur(2023);
        calculateur.CalculeLesCotisations(revenuNet);

        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(1870, 1m);
    }

    [Test]
    // Bug du simulateur officiel ? Les textes officiels parlent d'un taux progressif entre 40% et 110% du PASS (entre 0.5% et 0.85% de l'assiette) mais le simulateur reste bloqué sur 0.85%.
    public void Calculer_les_cotisations_pour_2023_pour_un_revenu_entre_40_pct_et_110_pct_du_PASS()
    {
        const decimal revenuNet = 36391m;
        var calculateur = Calculateurs.TrouveUnCalculateur(2023);
        calculateur.CalculeLesCotisations(revenuNet);

        // avec un taux progressif de 0.71% environ
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(260m, 1m);
    }
}