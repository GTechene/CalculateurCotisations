using Microsoft.Playwright;

namespace Cotisations.Tests.EndToEnd;

public static class PageExtensions
{
    public static async Task SubmitCotisationsForm(this IPage page, decimal revenuNet, int annee, decimal cotisationsFacultatives = 0m)
    {
        await page.FillAsync("#revenuNet", $"{revenuNet}");
        await page.SelectOptionAsync("#annee", $"{annee}");
        await Task.WhenAll(
            page.ClickAsync("#submitButton"),
            page.WaitForResponseAsync("**/cotisations/v2/precises/**")
        );
    }
}