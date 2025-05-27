using System.Globalization;
using Cotisations.Tests.EndToEnd.Extensions;
using Microsoft.Playwright.NUnit;
using NFluent;

namespace Cotisations.Tests.EndToEnd;

public class CotisationsPageShould : PageTest
{
    [Test]
    public async Task Affiche_les_cotisations_correctes_dans_le_tableau()
    {
        await using var api = new CotisationsApi();

        await Page.GotoAsync(api.ServerAddress);
        await Page.SubmitCotisationsForm(50000, 2024);
        
        var valeurCotisationsMaladie = await RecupereLaValeurDansLeTableau("Cotisations maladie (hors indemnités)");
        Check.That(valeurCotisationsMaladie).IsEqualTo(3483);
        var indemnitesMaladie = await RecupereLaValeurDansLeTableau("Cotisations indemnités maladie");
        Check.That(indemnitesMaladie).IsEqualTo(260);
        var retraiteDeBase = await RecupereLaValeurDansLeTableau("Retraite de base");
        Check.That(retraiteDeBase).IsEqualTo(8264);
        var retraiteComplementaire = await RecupereLaValeurDansLeTableau("Retraite complémentaire");
        Check.That(retraiteComplementaire).IsEqualTo(3729);
        var invaliditeDeces = await RecupereLaValeurDansLeTableau("Invalidité/décès");
        Check.That(invaliditeDeces).IsEqualTo(603);
        var allocationsFamiliales = await RecupereLaValeurDansLeTableau("Allocations familiales");
        Check.That(allocationsFamiliales).IsEqualTo(113);
        var csgDeductible = await RecupereLaValeurDansLeTableau("CSG déductible");
        Check.That(csgDeductible).IsEqualTo(4654);
        var csgNonDeductible = await RecupereLaValeurDansLeTableau("CSG non déductible");
        Check.That(csgNonDeductible).IsEqualTo(1642);
        var crds = await RecupereLaValeurDansLeTableau("CRDS");
        Check.That(crds).IsEqualTo(342);
        var formationProfessionnelle = await RecupereLaValeurDansLeTableau("Formation professionnelle");
        Check.That(formationProfessionnelle).IsEqualTo(116);
    }

    [Test]
    public async Task Met_les_bons_parametres_dans_l_url_du_navigateur()
    {
        await using var api = new CotisationsApi();

        await Page.GotoAsync(api.ServerAddress);
        await Page.SubmitCotisationsForm(50000, 2024);

        Check.That(Page.Url).EndsWith("?revenuNet=50000&annee=2024");
    }

    [Test]
    public async Task Deplie_l_accordeon_des_options_quand_les_cotisations_facultatives_sont_non_nulles_et_qu_on_met_les_params_dans_l_URL()
    {
        await using var api = new CotisationsApi();

        await Page.GotoAsync(api.ServerAddress + "?revenuNet=50000&annee=2024&cotisationsFacultatives=1500");

        var accordionButton = Page.Locator(".accordion-button").First;
        await Expect(accordionButton).Not.ToContainClassAsync("collapsed");
        await Expect(accordionButton).ToHaveAttributeAsync("aria-expanded", "true");
        var accordionCollapse = Page.Locator("#accordionCollapse").First;
        await Expect(accordionCollapse).ToContainClassAsync("show");
    }

    [Test]
    public async Task Ne_deplie_pas_l_accordeon_des_options_quand_les_cotisations_facultatives_sont_nulles_et_qu_on_met_les_params_dans_l_URL()
    {
        await using var api = new CotisationsApi();

        await Page.GotoAsync(api.ServerAddress + "?revenuNet=50000&annee=2024");

        var accordionButton = Page.Locator(".accordion-button").First;
        await Expect(accordionButton).ToContainClassAsync("collapsed");
        await Expect(accordionButton).ToHaveAttributeAsync("aria-expanded", "false");
        var accordionCollapse = Page.Locator("#accordionCollapse").First;
        await Expect(accordionCollapse).Not.ToContainClassAsync("show");
    }

    private async Task<decimal> RecupereLaValeurDansLeTableau(string nomDeLaLigne)
    {
        var cellules = await Page.Locator($"tr:has-text('{nomDeLaLigne}')").First.Locator("td").AllAsync();
        var valeurEnTexte = await cellules[1].TextContentAsync();
        var valeurSansLeSigneEuro = valeurEnTexte!.Replace("€", "");

        var valeur = decimal.Parse(valeurSansLeSigneEuro, CultureInfo.GetCultureInfo("fr-FR"));
        return valeur;
    }
}