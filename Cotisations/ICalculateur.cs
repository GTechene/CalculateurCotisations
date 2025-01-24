namespace Cotisations;

public interface ICalculateur
{
    public ResultatAvecExplicationEtTaux MaladieHorsIndemnitesJournalieres { get; }
    public ResultatAvecTauxUniqueEtExplication MaladieIndemnitesJournalieres { get; }
    public ResultatAvecTauxMultiplesEtExplication RetraiteDeBase { get; }
    public ResultatAvecTauxMultiplesEtExplication RetraiteComplementaire { get; }
    public ResultatAvecTauxUniqueEtExplication InvaliditeDeces { get; }
    public ResultatAvecTauxUniqueEtExplication AllocationsFamiliales { get; }
    public ResultatAvecTauxUniqueEtExplication CSGNonDeductible { get; }
    public ResultatAvecTauxUniqueEtExplication CSGDeductible { get; }
    public ResultatAvecTauxUniqueEtExplication CRDSNonDeductible { get; }
    public ResultatAvecTauxUniqueEtExplication FormationProfessionnelle { get; }
    public decimal TotalCotisationsObligatoires => MaladieHorsIndemnitesJournalieres.Valeur + MaladieIndemnitesJournalieres.Valeur + RetraiteDeBase.Valeur + RetraiteComplementaire.Valeur + InvaliditeDeces.Valeur + AllocationsFamiliales.Valeur;
    public decimal GrandTotal => TotalCotisationsObligatoires + CSGDeductible.Valeur + CSGNonDeductible.Valeur + CRDSNonDeductible.Valeur + FormationProfessionnelle.Valeur;
    public decimal AssietteDeCalculDesCotisations { get; }

    void CalculeLesCotisations(decimal revenu);
}