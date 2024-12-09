namespace Cotisations;

public record ResultatAvecExplication(decimal Valeur, string Explication);

public record ResultatAvecTauxUniqueEtExplication(decimal Valeur, string Explication, decimal TauxUnique) : ResultatAvecExplicationEtTaux(Valeur, Explication, TauxUnique);

public record ResultatVideSansExplication() : ResultatAvecTauxUniqueEtExplication(0m, string.Empty, 0m);

public record ResultatAvecTauxMultiplesEtExplication(decimal Valeur, string Explication, decimal Taux1, decimal Taux2) : ResultatAvecExplicationEtTaux(Valeur, Explication, Taux1, Taux2);

public record ResultatVideAvecTauxMultiplesEtSansExplication() : ResultatAvecTauxMultiplesEtExplication(0m, string.Empty, 0m, 0m);

public record ResultatAvecExplicationEtTaux(decimal Valeur, string Explication, params decimal[] Taux) : ResultatAvecExplication(Valeur, Explication);