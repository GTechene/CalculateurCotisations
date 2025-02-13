using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;
using Diverse;
using NFluent;

namespace Cotisations.Tests.Acceptance;

public class CotisationsApiV2Should
{
    private readonly VerifySettings _verifySettings = new();

    public CotisationsApiV2Should()
    {
        _verifySettings.UseDirectory("Snapshots");
    }

    [Test]
    public async Task Renvoyer_les_resultats_adequats_pour_des_cotisations_2025()
    {
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecRevenuNetDe(50000)
            .En(2025);

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.CalculeCotisationsPrecisesAvecExplications();

        await Verify(reponseHttp, _verifySettings);
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
        var xml = ExtractXmlWorksheet(stream);
        await Verify(xml, _verifySettings);
    }

    [Test]
    public async Task Telecharger_un_fichier_Excel_contenant_les_cotisations_2025()
    {
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecRevenuNetDe(60371)
            .AvecCotisationsFacultativesDe(1000m)
            .En(2025);

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.TelechargeExportExcel();

        Check.That(reponseHttp).IsOk();
        Check.That(reponseHttp.Content.Headers.ContentType).IsEqualTo(MediaTypeHeaderValue.Parse("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"));

        var stream = await reponseHttp.Content.ReadAsStreamAsync();
        var xml = ExtractXmlWorksheet(stream);
        await Verify(xml, _verifySettings);
    }

    private static string ExtractXmlWorksheet(Stream stream)
    {
        var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        var sheetEntry = archive.Entries.Single(entry => entry.FullName == "xl/worksheets/sheet1.xml");
        var sheetStream = sheetEntry.Open();
        var xml = new StreamReader(sheetStream, Encoding.UTF8).ReadToEnd();
        return xml;
    }

    [Test]
    public async Task Renvoyer_une_erreur_400_si_le_revenu_net_est_superieur_ou_egal_a_5_millions()
    {
        var fuzzer = new Fuzzer();
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecRevenuNetDe(fuzzer.GenerateInteger(5_000_000));

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.CalculeCotisationsPrecisesAvecExplications();

        Check.That(reponseHttp).IsBadRequest();
    }

    [Test]
    public async Task Renvoyer_une_erreur_400_si_le_revenu_net_est_inferieur_ou_egal_a_0()
    {
        var fuzzer = new Fuzzer();
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecRevenuNetDe(fuzzer.GenerateInteger(null, 0));

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.CalculeCotisationsPrecisesAvecExplications();

        Check.That(reponseHttp).IsBadRequest();
    }

    [Test]
    public async Task Renvoyer_une_erreur_400_si_l_annee_est_differente_de_2023_ou_2024()
    {
        var fuzzer = new Fuzzer();
        var annee = fuzzer.GenerateInteger(1);
        while(annee >= 2023 && annee <= DateTime.Today.Year)
            annee = fuzzer.GenerateInteger(1);

        var scenario = new ScenarioDeCotisationsPrecises().En(annee);

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.CalculeCotisationsPrecisesAvecExplications();

        Check.That(reponseHttp).IsBadRequest();
    }

    [Test]
    public async Task Renvoyer_une_erreur_400_si_les_cotisations_facultatives_sont_strictement_negatives()
    {
        var fuzzer = new Fuzzer();
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecCotisationsFacultativesDe(fuzzer.GenerateInteger(null, 0));

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.CalculeCotisationsPrecisesAvecExplications();

        Check.That(reponseHttp).IsBadRequest();
    }

    [Test]
    public async Task Renvoyer_une_erreur_400_si_le_revenu_net_est_superieur_ou_egal_a_5_millions_quand_on_exporte()
    {
        var fuzzer = new Fuzzer();
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecRevenuNetDe(fuzzer.GenerateInteger(5_000_000));

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.TelechargeExportExcel();

        Check.That(reponseHttp).IsBadRequest();
    }

    [Test]
    public async Task Renvoyer_une_erreur_400_si_le_revenu_net_est_inferieur_ou_egal_a_0_quand_on_exporte()
    {
        var fuzzer = new Fuzzer();
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecRevenuNetDe(fuzzer.GenerateInteger(null, 0));

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.TelechargeExportExcel();

        Check.That(reponseHttp).IsBadRequest();
    }

    [Test]
    public async Task Renvoyer_une_erreur_400_si_l_annee_est_differente_de_2023_ou_2024_quand_on_exporte()
    {
        var fuzzer = new Fuzzer();
        var annee = fuzzer.GenerateInteger(1);
        while (annee >= 2023 && annee <= DateTime.Today.Year)
        {
            annee = fuzzer.GenerateInteger(1);
        }

        var scenario = new ScenarioDeCotisationsPrecises().En(annee);

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.TelechargeExportExcel();

        Check.That(reponseHttp).IsBadRequest();
    }

    [Test]
    public async Task Renvoyer_une_erreur_400_si_les_cotisations_facultatives_sont_strictement_negatives_quand_on_exporte()
    {
        var fuzzer = new Fuzzer();
        var scenario = new ScenarioDeCotisationsPrecises()
            .AvecCotisationsFacultativesDe(fuzzer.GenerateInteger(null, 0));

        var api = CotisationsApi.CreeUneInstance(scenario);

        var reponseHttp = await api.TelechargeExportExcel();

        Check.That(reponseHttp).IsBadRequest();
    }
}