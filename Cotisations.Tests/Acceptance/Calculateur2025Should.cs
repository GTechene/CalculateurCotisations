using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NFluent;

namespace Cotisations.Tests.Acceptance;

public class Calculateur2025Should
{
    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_60_pct_et_110_pct_du_PASS()
    {
        var calculateur = new Calculateur2025();

        calculateur.CalculeLesCotisations(50_000m);

        Check.That(calculateur.Assiette).IsEqualTo(37000m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(3153m, 1m);
        //Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(8328m);
        //Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4996m);
        //Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(2905m, 1m);
        //Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1025m, 1m);
        //Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(214m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_inferieurs_a_20_pct_du_PASS()
    {
        var calculateur = new Calculateur2025();

        calculateur.CalculeLesCotisations(9_000m);

        Check.That(calculateur.Assiette).IsEqualTo(6660m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsEqualTo(0m);
        //Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(8328m);
        //Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4996m);
        //Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(2905m, 1m);
        //Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1025m, 1m);
        //Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(214m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_20_pct_et_40_pct_du_PASS()
    {
        var calculateur = new Calculateur2025();

        calculateur.CalculeLesCotisations(15_000m);

        Check.That(calculateur.Assiette).IsEqualTo(11100m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(133m, 1m);
        //Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(8328m);
        //Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4996m);
        //Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(2905m, 1m);
        //Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1025m, 1m);
        //Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(214m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_40_pct_et_60_pct_du_PASS()
    {
        var calculateur = new Calculateur2025();

        calculateur.CalculeLesCotisations(25_000m);

        Check.That(calculateur.Assiette).IsEqualTo(18500m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(784m, 1m);
        //Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(8328m);
        //Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4996m);
        //Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(2905m, 1m);
        //Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1025m, 1m);
        //Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(214m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_110_pct_et_200_pct_du_PASS()
    {
        var calculateur = new Calculateur2025();

        calculateur.CalculeLesCotisations(60_000m);

        Check.That(calculateur.Assiette).IsEqualTo(44400m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4039m, 1m);
        //Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(8328m);
        //Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4996m);
        //Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(2905m, 1m);
        //Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1025m, 1m);
        //Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(214m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_entre_200_pct_et_300_pct_du_PASS()
    {
        var calculateur = new Calculateur2025();

        calculateur.CalculeLesCotisations(120_000m);

        Check.That(calculateur.Assiette).IsEqualTo(88800m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(9766m, 1m);
        //Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(8328m);
        //Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4996m);
        //Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(2905m, 1m);
        //Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1025m, 1m);
        //Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(214m, 1m);
    }

    [Test]
    public void Calculer_les_cotisations_selon_un_exemple_simple_pour_des_revenus_au_dela_de_300_pct_du_PASS()
    {
        var calculateur = new Calculateur2025();

        calculateur.CalculeLesCotisations(200_000m);

        Check.That(calculateur.Assiette).IsEqualTo(148000m);
        Check.That(calculateur.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(15826m, 1m);
        //Check.That(calculateur.RetraiteDeBase.Valeur).IsEqualTo(8328m);
        //Check.That(calculateur.RetraiteComplementaire.Valeur).IsEqualTo(4996m);
        //Check.That(calculateur.CSGDeductible.Valeur).IsCloseTo(2905m, 1m);
        //Check.That(calculateur.CSGNonDeductible.Valeur).IsCloseTo(1025m, 1m);
        //Check.That(calculateur.CRDSNonDeductible.Valeur).IsCloseTo(214m, 1m);
    }
}