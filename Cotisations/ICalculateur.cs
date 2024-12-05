namespace Cotisations;

public interface ICalculateur
{
    public ResultatAvecTauxEtExplication MaladieHorsIndemnitesJournalieres { get; }
    public ResultatAvecTauxEtExplication MaladieIndemnitesJournalieres { get; }
    public ResultatAvecTauxMultiplesEtExplication RetraiteDeBase { get; }
    public ResultatAvecTauxMultiplesEtExplication RetraiteComplementaire { get; }
    public ResultatAvecTauxEtExplication InvaliditeDeces { get; }
    public ResultatAvecTauxEtExplication AllocationsFamiliales { get; }
    public ResultatAvecTauxEtExplication CSGNonDeductible { get; }
    public ResultatAvecTauxEtExplication CSGDeductible { get; }
    public ResultatAvecTauxEtExplication CRDSNonDeductible { get; }
    public ResultatAvecTauxEtExplication FormationProfessionnelle { get; }
    public decimal TotalCotisationsObligatoires => MaladieHorsIndemnitesJournalieres.Valeur + MaladieIndemnitesJournalieres.Valeur + RetraiteDeBase.Valeur + RetraiteComplementaire.Valeur + InvaliditeDeces.Valeur + AllocationsFamiliales.Valeur;
    public decimal GrandTotal => TotalCotisationsObligatoires + CSGDeductible.Valeur + CSGNonDeductible.Valeur + CRDSNonDeductible.Valeur + FormationProfessionnelle.Valeur;

    void CalculeLesCotisations(decimal revenu);
}