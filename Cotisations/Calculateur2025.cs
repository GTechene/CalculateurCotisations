using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cotisations;

public class Calculateur2025 : ICalculateur
{
    private const decimal TauxPlancherDeLAbattement = 0.0176m;
    private const decimal TauxPlafondDeLAbattement = 1.30m;
    private const decimal TauxAbattement = 0.26m;

    private readonly PlafondAnnuelSecuriteSociale _pass;

    public Calculateur2025()
    {
        _pass = new PlafondAnnuelSecuriteSociale(2025);
    }

    public void CalculeLesCotisations(decimal revenu)
    {
        var abattementPlancher = TauxPlancherDeLAbattement * _pass.Valeur;
        var abattementPlafond = TauxPlafondDeLAbattement * _pass.Valeur;
        var abattement = Math.Min(Math.Max(revenu * TauxAbattement, abattementPlancher),  abattementPlafond);

        Assiette = revenu - abattement;
    }

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
    public decimal Assiette { get; private set; }
}