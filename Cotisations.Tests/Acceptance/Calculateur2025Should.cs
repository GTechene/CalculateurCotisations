using NFluent;

namespace Cotisations.Tests.Acceptance;

public class Calculateur2025Should
{
    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_60_pct_et_110_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(50_000m, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.RevenuBrut).IsCloseTo(72939m, 1m);
        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(53975m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(3541m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(270m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(8466m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(4441m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(3670m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1295m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(270m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(612.3m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(256m, 1m);
        Check.That(calculateur.FormationProfessionnelle.Valeur).IsEqualTo(117.75m);

        // Teste que la culture est bien appliquée explicitement dans le code et ne dépend pas de la machine qui le fait tourner.
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Explication).IsEqualTo("L'assiette de 53 975 € est comprise entre 51 810 € (110% du PASS) et 94 200 € (2 PASS), donc un taux progressif entre 6,5% et 7,7% est appliqué. Ici il s'agit de 6,6%, soit 3 541 € de cotisations.");
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_inferieurs_a_20_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(9_000m, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(9384m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsEqualTo(0m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(94.2m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(1677m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(760m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(638m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(225m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(47m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(122m, 1m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_20_pct_et_40_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(15_000m, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(15655m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(155m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(94.2m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(2798m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(1268m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(1065m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(376m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(78.3m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(203.5m, 1m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_40_pct_et_60_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(25_000m, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(26699m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(957m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(133.5m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(4771m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(2163m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(1815.5m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(641m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(133.5m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(347m, 1m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_110_pct_et_200_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(60_000m, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(64845m, 2m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4455m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(324m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(8545m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(5430m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(4409m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1556m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(324m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(612.3m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(1854m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_200_pct_et_300_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(120_000m, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(123_621m, 2m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(10137m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(618m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(8968m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(10779m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(8406m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(2967m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(618m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(612.3m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(3832m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_au_dela_de_300_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(200_000m, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(210_214m, 2m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(16490m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(1051m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(9592m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(16673.4m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(14294m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(5045m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(1051m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(612.3m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(6517m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_au_dela_de_500_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(250_000m, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(272_887m, 2m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(20564m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(1177.5m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(10043m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(16673.4m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(18556m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(6549m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(1364m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(612.3m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(8459m, 1m);
    }
}