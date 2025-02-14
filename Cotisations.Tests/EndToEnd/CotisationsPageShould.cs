using Microsoft.Playwright.NUnit;

namespace Cotisations.Tests.EndToEnd;

public class CotisationsPageShould : PageTest
{
    private readonly VerifySettings _verifySettings = new();

    public CotisationsPageShould()
    {
        _verifySettings.UseDirectory("Snapshots");
    }

    [Test]
    public async Task Affiche_les_cotisations_correctes_dans_le_tableau()
    {
        await using var api = new CotisationsApi();

        await Page.GotoAsync(api.ServerAddress);
        await Page.SubmitCotisationsForm(50000, 2024);
        
        await Verify(Page, _verifySettings);
    }
}