namespace CotisationsApi.Controllers;

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