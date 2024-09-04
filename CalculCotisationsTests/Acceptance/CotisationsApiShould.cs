using CotisationsApi.Controllers;
using NFluent;
using NUnit.Framework;

namespace CalculCotisationsTests.Acceptance;

public class CotisationsApiShould
{
    [Test]
    public async Task Renvoyer_les_resultats_adequats_pour_mes_cotisations_2024()
    {
        var scenario = new ScenarioDeCotisationsPrecises().AvecRevenuNetDe(62441m);
        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.CalculeCotisationsPrecises(scenario.RevenuNet);

        Check.That(reponseHttp).IsOk<ResultatPrecisDeCotisations>()
            .WhichPayload(resultat =>
            {
                Check.That(resultat).IsNotNull();
                Check.That(resultat!.MaladieHorsIndemnitesJournalieres).IsCloseTo(4349.38m, 1m);
                Check.That(resultat.MaladieIndemnitesJournalieres).IsCloseTo(324.58m, 1m);
                Check.That(resultat.RetraiteDeBase).IsCloseTo(8341.61m, 1m);
                Check.That(resultat.RetraiteComplementaire).IsCloseTo(4763.83m, 1m);
                Check.That(resultat.InvaliditeDeces).IsCloseTo(602.78m, 1m);
                Check.That(resultat.AllocationsFamiliales).IsCloseTo(2012.4m, 1m);
                Check.That(resultat.TotalCotisationsObligatoires).IsCloseTo(20394m, 5m);
                Check.That(resultat.CSGNonDeductible).IsCloseTo(2047.46m, 1m);
                Check.That(resultat.CSGDeductible).IsCloseTo(5801.13m, 1m);
                Check.That(resultat.CRDS).IsCloseTo(426.55m, 1m);
                Check.That(resultat.FormationProfessionnelle).IsEqualTo(115.92m);
                Check.That(resultat.GrandTotal).IsCloseTo(28785m, 5m);
            });
    }

    //[Test]
    //[SetCulture("en-US")]
    //public async Task Renvoyer_les_resultats_adequats_pour_mes_cotisations_2023()
    //{
    //    var scenario = new ScenarioDeCotisationsPrecises()
    //        .AvecRevenuNetDe(67648.25m)
    //        .En(2023);
    //    var api = CotisationsApi.CreeUneInstance(scenario);

    //    var reponseHttp = await api.CalculeCotisationsPrecisesAvecExplications(scenario.RevenuNet, scenario.Annee);

    //    Check.That(reponseHttp).IsOk<ResultatPrecisDeCotisationsAvecExplications>()
    //        .WhichPayload(resultat =>
    //        {
    //            Check.That(resultat).IsNotNull();
    //            Check.That(resultat.InvaliditeDeces.Valeur).IsCloseTo(572m, 1m);
    //            Check.That(resultat!.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4349.38m, 1m);
    //            Check.That(resultat.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(324.58m, 1m);
    //            Check.That(resultat.RetraiteDeBase.Valeur).IsCloseTo(8341.61m, 1m);
    //            Check.That(resultat.RetraiteComplementaire.Valeur).IsCloseTo(4763.83m, 1m);
    //            Check.That(resultat.AllocationsFamiliales.Valeur).IsCloseTo(2012.4m, 1m);
    //            Check.That(resultat.TotalCotisationsObligatoires).IsCloseTo(20394m, 5m);
    //            Check.That(resultat.CSGNonDeductible.Valeur).IsCloseTo(2047.46m, 1m);
    //            Check.That(resultat.CSGDeductible.Valeur).IsCloseTo(5801.13m, 1m);
    //            Check.That(resultat.CRDS.Valeur).IsCloseTo(426.55m, 1m);
    //            Check.That(resultat.FormationProfessionnelle.Valeur).IsEqualTo(115.92m);
    //            Check.That(resultat.GrandTotal).IsCloseTo(28785m, 5m);
    //        });
    //}
}