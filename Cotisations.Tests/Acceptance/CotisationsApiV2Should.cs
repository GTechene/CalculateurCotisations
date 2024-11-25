using System.Net.Http.Headers;
using ClosedXML.Excel;
using Cotisations.Api.Controllers;
using NFluent;

namespace Cotisations.Tests.Acceptance;

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

        var reponseHttp = await api.CalculeCotisationsPrecisesAvecExplications();

        Check.That(reponseHttp).IsOk<ResultatPrecisDeCotisationsAvecExplications>()
            .WhichPayload(resultat =>
            {
                Check.That(resultat).IsNotNull();
                Check.That(resultat!.InvaliditeDeces.Valeur).IsCloseTo(572m, 1m);
                Check.That(resultat.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4479m, 1m);
                Check.That(resultat.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(600m, 1m);
                Check.That(resultat.RetraiteDeBase.Valeur).IsCloseTo(7967m, 1m);
                Check.That(resultat.RetraiteComplementaire.Valeur).IsCloseTo(5236m, 1m);
                Check.That(resultat.AllocationsFamiliales.Valeur).IsCloseTo(2187m, 1m);
                Check.That(resultat.TotalCotisationsObligatoires).IsCloseTo(21042m, 1m);
                Check.That(resultat.CSGNonDeductible.Valeur).IsCloseTo(2198m, 1m);
                Check.That(resultat.CSGDeductible.Valeur).IsCloseTo(6228m, 1m);
                Check.That(resultat.CRDS.Valeur).IsCloseTo(457m, 1m);
                Check.That(resultat.FormationProfessionnelle.Valeur).IsCloseTo(110m, 1m);
                Check.That(resultat.GrandTotal).IsCloseTo(30037m, 1m);
            });
    }

    [Test]
    /*
     * Ce test est raccord à la fois avec ma feuille Excel ET avec le simulateur officiel, à genre 1 ou 2 € près sur la somme totale.
     * Le calcul est bon, les résultats matchent : on est nickel sur 2024.
     */
    public async Task Renvoyer_les_resultats_adequats_pour_mes_cotisations_2024()
    {
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecRevenuNetDe(60371)
            .AvecCotisationsFacultativesDe(1000m)
            .En(2024);

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.CalculeCotisationsPrecisesAvecExplications();

        Check.That(reponseHttp).IsOk<ResultatPrecisDeCotisationsAvecExplications>()
            .WhichPayload(resultat =>
            {
                Check.That(resultat).IsNotNull();
                Check.That(resultat!.InvaliditeDeces.Valeur).IsCloseTo(603m, 1m);
                Check.That(resultat.MaladieHorsIndemnitesJournalieres.Valeur).IsCloseTo(4275m, 1m);
                Check.That(resultat.MaladieIndemnitesJournalieres.Valeur).IsCloseTo(319m, 1m);
                Check.That(resultat.RetraiteDeBase.Valeur).IsCloseTo(8335m, 1m);
                Check.That(resultat.RetraiteComplementaire.Valeur).IsCloseTo(4675m, 1m);
                Check.That(resultat.AllocationsFamiliales.Valeur).IsCloseTo(1820m, 1m);
                Check.That(resultat.TotalCotisationsObligatoires).IsCloseTo(20026m, 1m);
                Check.That(resultat.CSGNonDeductible.Valeur).IsCloseTo(2012m, 1m);
                Check.That(resultat.CSGDeductible.Valeur).IsCloseTo(5700m, 1m);
                Check.That(resultat.CRDS.Valeur).IsCloseTo(419m, 1m);
                Check.That(resultat.FormationProfessionnelle.Valeur).IsCloseTo(116m, 1m);
                Check.That(resultat.GrandTotal).IsCloseTo(28273m, 1m);
            });
    }

    [Test]
    public async Task Telecharger_un_fichier_Excel_contenant_les_cotisations_2024()
    {
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecRevenuNetDe(60371)
            .AvecCotisationsFacultativesDe(1000m)
            .En(2024);

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.TelechargeExportExcel();

        Check.That(reponseHttp).IsOk();
        Check.That(reponseHttp.Content.Headers.ContentType).IsEqualTo(MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

        var stream = await reponseHttp.Content.ReadAsStreamAsync();
        var workbook = new XLWorkbook(stream);
        Check.That(workbook.Worksheet("Cotisations")).IsNotNull();
    }
}