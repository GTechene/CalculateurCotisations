using CalculCotisations;
using NFluent;
using NUnit.Framework;

namespace CalculCotisationsTests.Unit;

public class CalculateurSimpleShould
{
    [Test]
    public void Calculer_cotises_maladie_avec_revenu_entre_60_pct_et_110_pct_du_PASS()
    {
        const decimal revenuNet = 40000m;

        var calculateur = new CalculateurSimple(revenuNet);
        calculateur.CalculeLesCotisations();

        var expectedCotisations = 2167m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres).IsCloseTo(expectedCotisations, 1m);
    }

    [Test]
    public void Calculer_cotises_maladie_avec_revenu_superieur_a_110_pct_du_PASS()
    {
        const int revenuNet = 60000;

        var calculateur = new CalculateurSimple(revenuNet);
        calculateur.CalculeLesCotisations();

        var expectedCotisations = revenuNet * 0.067m;
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres).IsEqualTo(expectedCotisations);
        var expectedIndemnites = revenuNet * 0.005m;
        Check.That(calculateur.MaladieIndemnitesJournalieres).IsEqualTo(expectedIndemnites);
    }

    [Test]
    public void Calculer_indemnites_journalieres_maladie_avec_revenu_superieur_a_232k()
    {
        const int revenuNet = 300000;

        var calculateur = new CalculateurSimple(revenuNet);
        calculateur.CalculeLesCotisations();

        var expectedIndemnites = PlafondAnnuelSecuriteSociale.PASS.Valeur500Pct * 0.005m;
        Check.That(calculateur.MaladieIndemnitesJournalieres).IsEqualTo(expectedIndemnites);
    }

    [Test]
    public void Calculer_la_retraite_de_base_avec_un_revenu_depassant_le_PASS()
    {
        const int revenuNet = 64913;

        var calculateur = new CalculateurSimple(revenuNet);
        calculateur.CalculeLesCotisations();

        Check.That(calculateur.RetraiteDeBase).IsEqualTo(8341.59m);
    }

    [Test]
    public void Calculer_la_retraite_complementaire_avec_un_revenu_depassant_le_PASS()
    {
        const decimal revenuNet = 64913m;

        var calculateur = new CalculateurSimple(revenuNet);
        calculateur.CalculeLesCotisations();

        Check.That(calculateur.RetraiteComplementaire).IsEqualTo(4763.58m);
    }

    [Test]
    public void Calculer_l_invalidite_deces()
    {
        const decimal revenuNet = 60000m;

        var calculateur = new CalculateurSimple(revenuNet);
        calculateur.CalculeLesCotisations();

        Check.That(calculateur.InvaliditeDeces).IsEqualTo(0.013m * PlafondAnnuelSecuriteSociale.PASS.Valeur);
    }

    [Test]
    public void Calculer_les_allocations_familiales_avec_un_revenu_depassant_140_pct_du_PASS()
    {
        const decimal revenuNet = 70000m;

        var calculateur = new CalculateurSimple(revenuNet);
        calculateur.CalculeLesCotisations();

        Check.That(calculateur.AllocationsFamiliales).IsEqualTo(0.031m * revenuNet);
    }

    [Test]
    public void Calculer_les_allocations_familiales_avec_un_revenu_entre_110_pct_et_140_pct_du_PASS()
    {
        var pass = PlafondAnnuelSecuriteSociale.PASS;
        var revenuNetPileAuMilieuDeLaDiffEntrPlafondEtPlancher = (pass.Valeur140Pct - pass.Valeur110Pct) / 2 + pass.Valeur110Pct;

        var calculateur = new CalculateurSimple(revenuNetPileAuMilieuDeLaDiffEntrPlafondEtPlancher);
        calculateur.CalculeLesCotisations();

        Check.That(calculateur.AllocationsFamiliales).IsEqualTo(0.0155m * revenuNetPileAuMilieuDeLaDiffEntrPlafondEtPlancher);
    }

    [Test]
    public void Calculer_la_CSG_et_la_CRDS()
    {
        const decimal revenuNet = 70000m;

        var calculateur = new CalculateurSimple(revenuNet);
        calculateur.CalculeLesCotisations();

        var revenuPrisEnComptePourCSGEtCRDS = revenuNet + calculateur.TotalCotisationsObligatoires;

        Check.That(calculateur.CSGNonDeductible).IsEqualTo(0.024m * revenuPrisEnComptePourCSGEtCRDS);
        Check.That(calculateur.CSGDeductible).IsEqualTo(0.068m * revenuPrisEnComptePourCSGEtCRDS);
        Check.That(calculateur.CRDSNonDeductible).IsEqualTo(0.005m * revenuPrisEnComptePourCSGEtCRDS);
    }

    [Test]
    public void Calculer_la_formation_professionnelle()
    {
        var revenuNet = new Random().Next(60000, 120000);
        var calculateur = new CalculateurSimple(revenuNet);
        calculateur.CalculeLesCotisations();

        Check.That(calculateur.FormationProfessionnelle).IsEqualTo(PlafondAnnuelSecuriteSociale.PASS.Valeur * Taux.CotisationsFormationProfessionnelle);
    }
}
