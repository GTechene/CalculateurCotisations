namespace Cotisations;

public record ResultatAvecExplication(decimal Valeur, string Explication);

public record ResultatAvecTauxEtExplication(decimal Valeur, decimal Taux, string Explication) : ResultatAvecExplication(Valeur, Explication);

public record ResultatVideSansExplication() : ResultatAvecTauxEtExplication(0m, 0m, string.Empty);