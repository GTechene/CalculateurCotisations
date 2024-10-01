namespace Cotisations.Front.Components.Data;

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