using NFluent;
using NUnit.Framework;
using CotisationsApi.Controllers;

namespace CalculCotisationsTests.Acceptance;

public class CotisationsApiV2Should
{
    [Test]
    public async Task Renvoyer_les_resultats_adequats_pour_mes_cotisations_2023()
    {
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecRevenuNetDe(65182m)
            .AvecCotisationsFacultativesDe(2710m)
            .En(2023);

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.CalculeCotisationsPrecisesAvecExplications(scenario.RevenuNet, scenario.Annee, scenario.CotisationsFacultatives);

        Check.That(reponseHttp).IsOk<ResultatPrecisDeCotisationsAvecExplications>()
            .WhichPayload(resultat =>
            {
                Check.That(resultat).IsNotNull();
                Check.That(resultat!.InvaliditeDeces.Valeur).IsCloseTo(572m, 1m);
                Check.That(resultat.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4473m, 1m);
                Check.That(resultat.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(598m, 1m);
                Check.That(resultat.RetraiteDeBase.Valeur).IsCloseTo(7967m, 1m);
                Check.That(resultat.RetraiteComplementaire.Valeur).IsCloseTo(5228m, 1m);
                Check.That(resultat.AllocationsFamiliales.Valeur).IsCloseTo(2184m, 1m);
                Check.That(resultat.TotalCotisationsObligatoires).IsCloseTo(21023m, 1m);
                Check.That(resultat.CSGNonDeductible.Valeur).IsCloseTo(2195m, 1m);
                Check.That(resultat.CSGDeductible.Valeur).IsCloseTo(6220m, 1m);
                Check.That(resultat.CRDS.Valeur).IsCloseTo(457m, 1m);
                Check.That(resultat.FormationProfessionnelle.Valeur).IsCloseTo(110m, 1m);
                Check.That(resultat.GrandTotal).IsCloseTo(30007m, 1m);
            });
    }
}