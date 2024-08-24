namespace CalculateurCotisationsFront.Components.Data;

public class CotisationsApiHttpClient
{
    private readonly HttpClient _httpClient;

    public CotisationsApiHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<ResultatPrecisDeCotisations?> GetCotisations(decimal revenuNet)
    {
        return _httpClient.GetFromJsonAsync<ResultatPrecisDeCotisations>($"/cotisations/precises/{revenuNet}");
    }
}

public record ResultatPrecisDeCotisations(
    decimal MaladieHorsIndemnitesJournalieres,
    decimal MaladieIndemnitesJournalieres,
    decimal RetraiteDeBase,
    decimal RetraiteComplementaire,
    decimal InvaliditeDeces,
    decimal AllocationsFamiliales,
    decimal TotalCotisationsObligatoires,
    decimal CSGNonDeductible,
    decimal CSGDeductible,
    decimal CRDS,
    decimal FormationProfessionnelle,
    decimal GrandTotal);


public record CotisationsApiOptions
{
    public const string SectionName = "CotisationsApi";

    public Uri Uri { get; set; } = null!;
}