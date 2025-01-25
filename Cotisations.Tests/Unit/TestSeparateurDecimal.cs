using Microsoft.AspNetCore.Mvc.Testing;

namespace Cotisations.Tests.Unit;

internal class TestSeparateurDecimal
{
    [Test]
    [SetCulture("fr-FR")]
    public async Task Ne_pas_peter_quand_on_utilise_Verify_pour_checker_une_chaine_en_format_C0_en_culture_FR()
    {
        const decimal input = 1000.31m;
        await Verify($"{input:C0}");
    }

    [Test]
    [SetCulture("fr-FR")]
    public async Task Ne_pas_peter_quand_on_utilise_Verify_pour_checker_une_API_qui_renvoie_une_chaine_en_format_C0_en_culture_FR()
    {
        var api = new WebApplicationFactory<Program>().CreateClient();
        var response = await api.GetAsync("/test/separator");

        await Verify(response);
    }

    [Test]
    [SetCulture("fr-FR")]
    public async Task Ne_pas_peter_quand_on_utilise_Verify_pour_checker_une_API_qui_renvoie_un_JSON_contenant_une_chaine_en_format_C0_en_culture_FR()
    {
        var api = new WebApplicationFactory<Program>().CreateClient();
        var response = await api.GetAsync("/test/separator/json");

        await Verify(response);
    }
}
