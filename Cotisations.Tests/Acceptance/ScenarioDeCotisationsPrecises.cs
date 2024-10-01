using sas.Scenario;

namespace Cotisations.Tests.Acceptance;

public class ScenarioDeCotisationsPrecises : BaseScenario
{
    public decimal RevenuNet { private set; get; }
    public decimal CotisationsFacultatives { private set; get; }
    public int Annee { get; private set; } = DateTime.Today.Year;

    public ScenarioDeCotisationsPrecises AvecRevenuNetDe(decimal revenuNet)
    {
        RevenuNet = revenuNet;
        return this;
    }

    public ScenarioDeCotisationsPrecises AvecCotisationsFacultativesDe(decimal cotisationsFacultatives)
    {
        CotisationsFacultatives = cotisationsFacultatives;
        return this;
    }

    public ScenarioDeCotisationsPrecises En(int annee)
    {
        Annee = annee;
        return this;
    }
}