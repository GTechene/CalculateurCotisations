namespace Cotisations.Api.Controllers;

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

public record ResultatPrecisDeCotisationsAvecExplications(
    ResultatAvecExplication MaladieHorsIndemnitesJournalieres,
    ResultatAvecExplication MaladieIndemnitesJournalieres,
    ResultatAvecExplication RetraiteDeBase,
    ResultatAvecExplication RetraiteComplementaire,
    ResultatAvecExplication InvaliditeDeces,
    ResultatAvecExplication AllocationsFamiliales,
    decimal TotalCotisationsObligatoires,
    ResultatAvecExplication CSGNonDeductible,
    ResultatAvecExplication CSGDeductible,
    ResultatAvecExplication CRDS,
    ResultatAvecExplication FormationProfessionnelle,
    decimal GrandTotal);