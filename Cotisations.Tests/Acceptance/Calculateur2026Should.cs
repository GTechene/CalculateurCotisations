using NFluent;

namespace Cotisations.Tests.Acceptance;

public class Calculateur2026Should
{
    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_60_pct_et_110_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(50_000m, 2026);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(54000m, 5m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(3526m, 2m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(270m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(8631m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(4433m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(3672m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1296m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(270m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(624.78m, 1m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(130m, 3m);
        Check.That(calculateur.FormationProfessionnelle.Valeur).IsEqualTo(120.15m);

        // Teste que la culture est bien appliquée explicitement dans le code et ne dépend pas de la machine qui le fait tourner.
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Explication).IsEqualTo("L'assiette de 54 004 € est comprise entre 52 866 € (110% du PASS) et 96 120 € (2 PASS), donc un taux progressif entre 6,5% et 7,7% est appliqué. Ici il s'agit de 6,5%, soit 3 527 € de cotisations.");
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_inferieurs_a_20_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(9_000m, 2026);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(9388m, 2m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsEqualTo(0m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(96.12m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(1678m, 1m);
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
        var calculateur = new CalculateurAvecConvergence(15_000m, 2026);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(15650m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(147m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(96m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(2797m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(1268m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(1064m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(376m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(78m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(203m, 1m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_40_pct_et_60_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(25_000m, 2026);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(26657m, 2m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(914m, 2m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(133m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(4764m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(2159m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(1813m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(640m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(133m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(347m, 1m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_110_pct_et_200_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(60_000m, 2026);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(64803m, 4m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4426m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(324m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(8709m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(5416m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(4407m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1555m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(324m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(624.78m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(1665m, 3m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_200_pct_et_300_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(120_000m, 2026);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(123_736m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(10097m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(619m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(9133m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(10779m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(8414m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(2970m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(619m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(624.78m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(3836m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_au_dela_de_300_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(200_000m, 2026);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(209_367m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(16492m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(1047m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(9749m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(17013m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(14237m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(5025m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(1047m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(624.78m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(6490m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_au_dela_de_500_pct_du_PASS()
    {
        var calculateur = new CalculateurAvecConvergence(250_000m, 2026);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(272_077m, 1m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(20569m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(1201.5m, 1m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(10201m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsCloseTo(17013m, 1m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(18501m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(6530m, 1m);
        Check.That(calculateur.CRDS.Valeur).IsCloseTo(1360m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(624.78m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(8434m, 1m);
    }
}