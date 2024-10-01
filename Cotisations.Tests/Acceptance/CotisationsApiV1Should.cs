using Cotisations.Api.Controllers;
using NFluent;
using NUnit.Framework;

namespace Cotisations.Tests.Acceptance;

public class CotisationsApiV1Should
{
    [Test]
    public async Task Renvoyer_les_resultats_adequats_pour_mes_cotisations_2024_sans_explication()
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
}