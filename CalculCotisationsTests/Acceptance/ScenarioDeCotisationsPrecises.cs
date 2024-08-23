using sas.Scenario;

namespace CalculCotisationsTests.Acceptance;

public class ScenarioDeCotisationsPrecises : BaseScenario
{
    public decimal RevenuNet { private set; get; }

    public ScenarioDeCotisationsPrecises AvecRevenuNetDe(decimal revenuNet)
    {
        RevenuNet = revenuNet;
        return this;
    }
}