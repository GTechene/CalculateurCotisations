namespace CalculateurCotisationsFront.Components.Data;

public class CotisationsApiHttpClient
{
    private readonly HttpClient _httpClient;

    public CotisationsApiHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<ResultatPrecisDeCotisationsAvecExplications?> GetCotisations(decimal revenuNet, int annee)
    {
        return _httpClient.GetFromJsonAsync<ResultatPrecisDeCotisationsAvecExplications>($"/cotisations/v2/precises/{revenuNet}?annee={annee}");
    }
}