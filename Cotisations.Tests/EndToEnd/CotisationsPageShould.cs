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

    private async Task<decimal> RecupereLaValeurDansLeTableau(string nomDeLaLigne)
    {
        var cellules = await Page.Locator($"tr:has-text('{nomDeLaLigne}')").First.Locator("td").AllAsync();
        var valeurEnTexte = await cellules[1].TextContentAsync();
        var valeur = decimal.Parse(valeurEnTexte!.Replace("€", "").Replace(" ", ""));
        return valeur;
    }
}