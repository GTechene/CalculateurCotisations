using sas.Api;
using sas.Configurations;
using sas.Simulators;

namespace Cotisations.Tests.Acceptance;

public class CotisationsApi : BaseApi<Program>
{
    private readonly ScenarioDeCotisationsPrecises _scenario;

    public static CotisationsApi CreeUneInstance(ScenarioDeCotisationsPrecises scenario)
    {
        return new CotisationsApi(scenario, [], []);
    }

    protected CotisationsApi(ScenarioDeCotisationsPrecises scenario, ISimulateBehaviour[] simulators, IEnrichConfiguration[] configurations) : base(scenario, simulators, configurations)
    {
        _scenario = scenario;
    }

    [Obsolete($"Utiliser {nameof(CalculeCotisationsPrecisesAvecExplications)}")]
    public async Task<HttpResponseMessage> CalculeCotisationsPrecises()
    {
        return await HttpClient.GetAsync($"/cotisations/precises/{_scenario.RevenuNet}");
    }

    public async Task<HttpResponseMessage> CalculeCotisationsPrecisesAvecExplications()
    {
        var requestUri = $"/cotisations/v2/precises/{_scenario.RevenuNet}?annee={_scenario.Annee}&cotisationsFacultatives={_scenario.CotisationsFacultatives}";
        return await HttpClient.GetAsync(requestUri);
    }

    public async Task<HttpResponseMessage> TelechargeExportExcel()
    {
        var requestUri = $"/cotisations/export/{_scenario.RevenuNet}?annee={_scenario.Annee}&cotisationsFacultatives={_scenario.CotisationsFacultatives}";
        return await HttpClient.GetAsync(requestUri);
    }
}