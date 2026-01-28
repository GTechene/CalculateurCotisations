using ClosedXML.Excel;
using System.Globalization;

namespace Cotisations.Excel;

public class ExporteurExcel
{
    private readonly ICalculateur _calculateur;
    private readonly IConstantesAvecHistorique _constantesAvecHistorique;
    private readonly PlafondAnnuelSecuriteSociale _pass;
    private readonly int _annee;
    private readonly decimal _revenu;
    private readonly decimal _cotisationsFacultatives;

    public ExporteurExcel(ICalculateur calculateur, int annee, decimal revenu, decimal cotisationsFacultatives)
    {
        _calculateur = calculateur;
        _annee = annee;
        _revenu = revenu;
        _cotisationsFacultatives = cotisationsFacultatives;
        _constantesAvecHistorique = ConstantesAvecHistorique.PourLAnnee(_annee);
        _pass = new PlafondAnnuelSecuriteSociale(_annee);

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    public void Exporte(Stream output)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Cotisations");
        
        worksheet.Cell("A1").Value = "Revenu net";
        worksheet.Cell("B1").WithImportantValue(_revenu);

        worksheet.Cell("A2").Value = "Cotisations facultatives";
        worksheet.Cell("B2").Value = _cotisationsFacultatives;

        if (_annee < 2025)
        {
            worksheet.Cell("A4").Value = "Assiette (obtenue par convergence)";
            worksheet.Cell("B4")
                .WithImportantValue(_calculateur.AssietteDeCalculDesCotisations)
                .WithPlainNumberFormat()
                .WithLargeComment("Cette valeur est calculée en calculant d'une part les cotisations sur une assiette approximative puis, après calcul de la CSG et la CRDS, de la comparaison entre cette première assiette et une deuxième qui est la somme entre le revenu net, la CSG non déductible et la CRDS. En cas d'écart, on modifie la première assiette pour se rapprocher de la seconde et on itère jusqu'à ce que la différence entre les deux soit inférieure à 1 €.");
        }
        else
        {
            worksheet.Cell("A4").Value = "Assiette";
            worksheet.Cell("B4")
                .WithImportantValue(_calculateur.AssietteDeCalculDesCotisations)
                .WithPlainNumberFormat();
        }
        

        worksheet.Cell("A5").Value = "PASS";
        worksheet.Cell("B5")
            .SetValue(_pass.Valeur)
            .WithShortComment($"Plafond Annuel de la Sécurité Sociale pour l'année {_annee}");

        worksheet.Cell("A6").Value = "Plafond retraite complémentaire";
        if (_annee < 2025)
            worksheet.Cell("B6").Value = _constantesAvecHistorique.PlafondsRetraiteComplementaireArtisansCommercants;
        else
            worksheet.Cell("B6").Value = _pass.Valeur;


        var (celluleCotisationsMaladie, celluleIndemnites) = ExporteLesCotisationsMaladie(worksheet);
        var (celluleRetraiteDeBase, celluleRetraiteComplementaire) = ExporteLesCotisationsRetraite(worksheet);
        var celluleInvaliditeDeces = ExporteLesCotisationsInvaliditeDeces(worksheet);
        var celluleAllocs = ExporteLesCotisationsAllocationsFamiliales(worksheet);

        worksheet.Cell("A18").WithImportantValue("Total cotisations obligatoires");
        worksheet.Cell("B18")
            .WithImportantFormula($"={celluleCotisationsMaladie}+{celluleIndemnites}+{celluleRetraiteDeBase}+{celluleRetraiteComplementaire}+{celluleInvaliditeDeces}+{celluleAllocs}")
            .WithPlainNumberFormat();

        var celluleFormationPro = ExporteLesCotisationsFormationProfessionnelle(worksheet);

        var (celluleCsgDeductible, celluleCsgNonDeductible, celluleCrds) = ExporteCsgEtCrds(worksheet);

        worksheet.Cell("A28").WithImportantValue("Total cotisations");
        worksheet.Cell("B28")
            .WithImportantFormula($"=B18+{celluleFormationPro}+{celluleCsgDeductible}+{celluleCsgNonDeductible}+{celluleCrds}")
            .WithPlainNumberFormat();
        worksheet.Cell("A29").Value = "Part du revenu";
        worksheet.Cell("B29")
            .SetFormulaA1("=B28/B1")
            .Style.NumberFormat.SetFormat("0.0%");

        worksheet.Columns().AdjustToContents();

        worksheet.Cell("A32")
            .SetValue("Taux et barêmes de cotisations sur le site de l'URSSAF")
            .SetHyperlink(new XLHyperlink("https://www.urssaf.fr/accueil/outils-documentation/taux-baremes/taux-cotisations-ac-plnr.html"));

        workbook.SaveAs(output);
    }

    private (string celluleCotisationsMaladie, string celluleIndemnites) ExporteLesCotisationsMaladie(IXLWorksheet worksheet)
    {
        worksheet.Cell("A8").Value = "Cotisations maladie hors indemnités";
        const string celluleCotisationsMaladie = "B8";
        worksheet.Cell(celluleCotisationsMaladie)
            // TODO: Cotisation 2025, il y aura plusieurs taux si revenu > 3 PASS :) Il faudra un formatage différent
            .WithImportantFormula($"=B4*{_calculateur.MaladieHorsIndemnitesJournalieres.Taux.Single()}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.MaladieHorsIndemnitesJournalieres.Explication);

        worksheet.Cell("A9").Value = "Cotisations indemnités maladie";
        const string celluleIndemnites = "B9";
        worksheet.Cell(celluleIndemnites)
            .WithImportantFormula($"=B4*{_calculateur.MaladieIndemnitesJournalieres.TauxUnique}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.MaladieIndemnitesJournalieres.Explication);

        return (celluleCotisationsMaladie, celluleIndemnites);
    }

    private (string celluleRetraiteDeBase, string celluleRetraiteComplementaire) ExporteLesCotisationsRetraite(IXLWorksheet worksheet)
    {
        worksheet.Cell("A12").Value = "Retraite de base";
        worksheet.Cell("B11").Value = "Revenus tranche 1";
        worksheet.Cell("B12")
            .SetFormulaA1("=MIN(B4,B5)")
            .WithPlainNumberFormat();
        worksheet.Cell("C11").Value = "Taux 1";
        worksheet.Cell("C12")
            .SetValue(_calculateur.RetraiteDeBase.Taux1)
            .WithPercentFormat();

        string celluleRetraiteDeBase;
        string celluleRetraiteComplementaire;

        worksheet.Cell("A13").Value = "Retraite complémentaire";
        worksheet.Cell("B13")
            .SetFormulaA1("=MIN(B4,B6)")
            .WithPlainNumberFormat();
        worksheet.Cell("C13")
            .SetValue(_calculateur.RetraiteComplementaire.Taux1)
            .WithPercentFormat();

        if (_calculateur.AssietteDeCalculDesCotisations > _pass.Valeur)
        {
            celluleRetraiteDeBase = "F12";
            celluleRetraiteComplementaire = "F13";

            worksheet.Cell("D11").Value = "Revenus tranche 2";
            worksheet.Cell("D12")
                .SetFormulaA1("=B4-B5")
                .WithPlainNumberFormat();
            worksheet.Cell("E11").Value = "Taux 2";
            worksheet.Cell("E12")
                .SetValue(_calculateur.RetraiteDeBase.Taux2)
                .WithPercentFormat();

            worksheet.Cell("F11").Value = "Cotisation";
            worksheet.Cell(celluleRetraiteDeBase)
                .WithImportantFormula("=B12*C12+D12*E12")
                .WithPlainNumberFormat()
                .WithLargeComment(_calculateur.RetraiteDeBase.Explication);

            worksheet.Cell("D13")
                .SetFormulaA1("=B4-B6")
                .WithPlainNumberFormat();
            worksheet.Cell("E13")
                .SetValue(_calculateur.RetraiteComplementaire.Taux2)
                .WithPercentFormat();
                
            worksheet.Cell(celluleRetraiteComplementaire)
                .WithImportantFormula("=B13*C13+D13*E13")
                .WithPlainNumberFormat()
                .WithLargeComment(_calculateur.RetraiteComplementaire.Explication);
        }
        else
        {
            celluleRetraiteDeBase = "D12";
            celluleRetraiteComplementaire = "D13";

            worksheet.Cell("D11").Value = "Cotisation";
            worksheet.Cell(celluleRetraiteDeBase)
                .WithImportantFormula("=B12*C12")
                .WithPlainNumberFormat()
                .WithLargeComment(_calculateur.RetraiteDeBase.Explication);
                
            worksheet.Cell(celluleRetraiteComplementaire)
                .WithImportantFormula("=B13*C13")
                .WithPlainNumberFormat()
                .WithLargeComment(_calculateur.RetraiteComplementaire.Explication);
        }

        return (celluleRetraiteDeBase, celluleRetraiteComplementaire);
    }

    private string ExporteLesCotisationsInvaliditeDeces(IXLWorksheet worksheet)
    {
        worksheet.Cell("A15").Value = "Cotisations invalidité/décès";
        const string celluleResultat = "B15";
        worksheet.Cell(celluleResultat)
            .WithImportantFormula($"=MIN(B4,B5)*{_calculateur.InvaliditeDeces.TauxUnique}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.InvaliditeDeces.Explication);

        return celluleResultat;
    }

    private string ExporteLesCotisationsAllocationsFamiliales(IXLWorksheet worksheet)
    {
        worksheet.Cell("A16").Value = "Cotisations allocations familiales";
        const string celluleResultat = "B16";
        worksheet.Cell(celluleResultat)
            .WithImportantFormula($"=B4*{_calculateur.AllocationsFamiliales.TauxUnique}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.AllocationsFamiliales.Explication);

        return celluleResultat;
    }

    private string ExporteLesCotisationsFormationProfessionnelle(IXLWorksheet worksheet)
    {
        worksheet.Cell("A20").Value = "Formation professionnelle";
        const string celluleResultat = "B20";
        worksheet.Cell(celluleResultat)
            .WithImportantFormula($"=B5*{_calculateur.FormationProfessionnelle.TauxUnique}")
            .WithPlainNumberFormat()
            .WithShortComment(_calculateur.FormationProfessionnelle.Explication);

        return celluleResultat;
    }

    private (string celluleCsgDeductible, string celluleCsgNonDeductible, string celluleCrds) ExporteCsgEtCrds(IXLWorksheet worksheet)
    {
        if(_annee < 2025)
        {
            worksheet.Cell("A22").Value = "Revenus pris en compte pour CSG/CRDS";
            worksheet.Cell("B22")
                .SetFormulaA1("=B4+B18")
                .WithPlainNumberFormat();
        }
        else
        {
            worksheet.Cell("A22").Value = "Assiette CSG/CRDS";
            worksheet.Cell("B22")
                .SetFormulaA1("=B4")
                .WithPlainNumberFormat()
                // TODO : modifier le commentaire quand l'abattement est plafonné ou "planchisé"
                .WithShortComment("Assiette unique égale au revenu brut abattu forfaitairement de 26%");
        }


        worksheet.Cell("A24").Value = "CSG déductible";
        const string celluleCsgDeductible = "B24";
        worksheet.Cell(celluleCsgDeductible)
            .WithImportantFormula($"=B22*{_calculateur.CSGDeductible.TauxUnique}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.CSGDeductible.Explication);

        worksheet.Cell("A25").Value = "CSG non déductible";
        const string celluleCsgNonDeductible = "B25";
        worksheet.Cell(celluleCsgNonDeductible)
            .WithImportantFormula($"=B22*{_calculateur.CSGNonDeductible.TauxUnique}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.CSGNonDeductible.Explication);

        worksheet.Cell("A26").Value = "CRDS";
        const string celluleCrds = "B26";
        worksheet.Cell(celluleCrds)
            .WithImportantFormula($"=B22*{_calculateur.CRDSNonDeductible.TauxUnique}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.CRDSNonDeductible.Explication);

        return (celluleCsgDeductible, celluleCsgNonDeductible, celluleCrds);
    }
}