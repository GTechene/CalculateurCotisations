using NFluent;

namespace Cotisations.Tests.Acceptance;

public class Calculateur2026Should
{
    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_60_pct_et_110_pct_du_PASS()
    {
        var calculateur = new Calculateur2026();

        calculateur.CalculeLesCotisations(50_000m);

        Check.That(calculateur.AssietteCsgCrds).IsEqualTo(37000m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(3101m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(250m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(8728m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4069.4m);
        Check.That(calculateur.CSGDeductible.Valeur).IsEqualTo(2516m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsEqualTo(888m);
        Check.That(calculateur.CRDSNonDeductible.Valeur).IsEqualTo(185m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(624.78m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
        Check.That(calculateur.FormationProfessionnelle.Valeur).IsEqualTo(120.15m);

        // Teste que la culture est bien appliquée explicitement dans le code et ne dépend pas de la machine qui le fait tourner.
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Explication).IsEqualTo("L'assiette de 50 000 € est comprise entre 28 836 € (60% du PASS) et 52 866 € (110% du PASS), donc un taux progressif entre 4% et 6,5% est appliqué. Ici il s'agit de 6,2%, soit 3 101 € de cotisations.");
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_inferieurs_a_20_pct_du_PASS()
    {
        var calculateur = new Calculateur2026();

        calculateur.CalculeLesCotisations(9_000m);

        Check.That(calculateur.AssietteCsgCrds).IsEqualTo(6660m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsEqualTo(0m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(96.12m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(1608.3m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(729m);
        Check.That(calculateur.CSGDeductible.Valeur).IsEqualTo(452.88m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(160m, 1m);
        Check.That(calculateur.CRDSNonDeductible.Valeur).IsEqualTo(33.3m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(117m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_20_pct_et_40_pct_du_PASS()
    {
        var calculateur = new Calculateur2026();

        calculateur.CalculeLesCotisations(15_000m);

        Check.That(calculateur.AssietteCsgCrds).IsEqualTo(11100m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(126m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(96.12m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(2680.5m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(1215m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(755m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(266m, 1m);
        Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(55.5m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(195m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_40_pct_et_60_pct_du_PASS()
    {
        var calculateur = new Calculateur2026();

        calculateur.CalculeLesCotisations(25_000m);

        Check.That(calculateur.AssietteCsgCrds).IsEqualTo(18500m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(751m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(125m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(4467.5m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(2025m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(1258m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(444m, 1m);
        Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(92.5m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsCloseTo(325m, 1m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(0m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_110_pct_et_200_pct_du_PASS()
    {
        var calculateur = new Calculateur2026();

        calculateur.CalculeLesCotisations(60_000m);

        Check.That(calculateur.AssietteCsgCrds).IsEqualTo(44400m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4019m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(300m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(9448m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4979.4m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(3019.2m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1065.6m, 1m);
        Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(222m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(624.78m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsCloseTo(920m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_200_pct_et_300_pct_du_PASS()
    {
        var calculateur = new Calculateur2026();

        calculateur.CalculeLesCotisations(120_000m);

        Check.That(calculateur.AssietteCsgCrds).IsEqualTo(88800m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(9717m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(600m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(13768m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(10439.4m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(6038.4m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(2131.2m, 1m);
        Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(444m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(624.78m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(3720m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_au_dela_de_300_pct_du_PASS()
    {
        var calculateur = new Calculateur2026();

        calculateur.CalculeLesCotisations(200_000m);

        Check.That(calculateur.AssietteCsgCrds).IsEqualTo(148000m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(15883.6m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(1000m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(19528m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(17013.24m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(10064m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(3552m, 1m);
        Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(740m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(624.78m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(6200m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_au_dela_de_500_pct_du_PASS()
    {
        var calculateur = new Calculateur2026();

        calculateur.CalculeLesCotisations(250_000m);

        Check.That(calculateur.AssietteCsgCrds).IsEqualTo(187522m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(19133.6m, 1m);
        Check.That(calculateur.MaladieIndemnitesJournalieres.Valeur).IsEqualTo(1201.5m);
        Check.That(calculateur.RetraiteDeBase.Valeur).IsCloseTo(23128m, 1m);
        Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(17013.24m);
        Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(12751.5m, 1m);
        Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(4500.53m, 1m);
        Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(937.61m, 1m);
        Check.That(calculateur.InvaliditeDeces.Valeur).IsEqualTo(624.78m);
        Check.That(calculateur.AllocationsFamiliales.Valeur).IsEqualTo(7750m);
    }
}