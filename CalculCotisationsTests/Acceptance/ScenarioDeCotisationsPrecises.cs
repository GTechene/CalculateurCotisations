using sas.Scenario;

namespace CalculCotisationsTests.Acceptance;

public class ScenarioDeCotisationsPrecises : BaseScenario
{
    public decimal RevenuNet { private set; get; }
    public int Annee { get; private set; } = DateTime.Today.Year;

    public ScenarioDeCotisationsPrecises AvecRevenuNetDe(decimal revenuNet)
    {
        RevenuNet = revenuNet;
        return this;
    }

    public ScenarioDeCotisationsPrecises En(int annee)
    {
        Annee = annee;
        return this;
    }
}