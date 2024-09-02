namespace CalculCotisations;

public record ResultatAvecExplication(decimal Valeur, string Explication);

public record ResultatVideSansExplication() : ResultatAvecExplication(0m, string.Empty);