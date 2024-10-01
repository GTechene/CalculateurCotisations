namespace Cotisations.Front.Components.Data;

public record CotisationsApiOptions
{
    public const string SectionName = "CotisationsApi";

    public Uri Uri { get; set; } = null!;
}