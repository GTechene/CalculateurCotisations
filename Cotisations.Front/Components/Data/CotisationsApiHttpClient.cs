namespace Cotisations.Front.Components.Data;

public class CotisationsApiHttpClient
{
    private readonly HttpClient _httpClient;

    public CotisationsApiHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<ResultatPrecisDeCotisationsAvecExplications?> GetCotisations(decimal revenuNet, int annee, decimal cotisationsFacultatives = 0m)
    {
        return _httpClient.GetFromJsonAsync<ResultatPrecisDeCotisationsAvecExplications>($"/cotisations/v2/precises/{revenuNet}?annee={annee}&cotisationsFacultatives={cotisationsFacultatives}");
    }

    public Task<Stream> GetExcelFile(decimal revenuNet, int annee, decimal cotisationsFacultatives = 0m)
    {
        return _httpClient.GetStreamAsync($"/cotisations/export/{revenuNet}?annee={annee}&cotisationsFacultatives={cotisationsFacultatives}");
    }
}