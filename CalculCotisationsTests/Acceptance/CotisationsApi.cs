using sas.Api;
using sas.Configurations;
using sas.Scenario;
using sas.Simulators;

namespace CalculCotisationsTests.Acceptance;

public class CotisationsApi : BaseApi<Program>
{
    public static CotisationsApi CreeUneInstance(ScenarioDeCotisationsPrecises scenario)
    {
        return new CotisationsApi(scenario, [], []);
    }

    protected CotisationsApi(BaseScenario scenario, ISimulateBehaviour[] simulators, IEnrichConfiguration[] configurations) : base(scenario, simulators, configurations)
    {
    }

    [Obsolete($"Utiliser {nameof(CalculeCotisationsPrecisesAvecExplications)}")]
    public async Task<HttpResponseMessage> CalculeCotisationsPrecises(decimal revenuNet)
    {
        return await HttpClient.GetAsync($"/cotisations/precises/{revenuNet}");
    }

    public async Task<HttpResponseMessage> CalculeCotisationsPrecisesAvecExplications(decimal revenuNet, int annee, decimal cotisationsFacultatives)
    {
        var requestUri = $"/cotisations/v2/precises/{revenuNet}?annee={annee}&cotisationsFacultatives={cotisationsFacultatives}";
        return await HttpClient.GetAsync(requestUri);
    }
}