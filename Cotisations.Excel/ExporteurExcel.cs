﻿using ClosedXML.Excel;
using System.Globalization;

namespace Cotisations.Excel;

public class ExporteurExcel
{
    private readonly CalculateurAvecConvergence _calculateur;
    private readonly IConstantesAvecHistorique _constantesAvecHistorique;
    private readonly PlafondAnnuelSecuriteSociale _pass;

    public ExporteurExcel(CalculateurAvecConvergence calculateur)
    {
        _calculateur = calculateur;
        _constantesAvecHistorique = ConstantesAvecHistorique.PourLAnnee(_calculateur.Annee);
        _pass = new PlafondAnnuelSecuriteSociale(_calculateur.Annee);

        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }


    public void Exporte(Stream output)
    {
        var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Cotisations");
        
        worksheet.Cell("A1").Value = "Revenu net";
        worksheet.Cell("B1").WithImportantValue(_calculateur.RevenuNet);

        worksheet.Cell("A2").Value = "Cotisations facultatives";
        worksheet.Cell("B2").Value = _calculateur.CotisationsFacultatives;

        worksheet.Cell("A4").Value = "Assiette (obtenue par convergence)";
        worksheet.Cell("B4")
            .WithImportantValue(_calculateur.AssietteDeCalculDesCotisations)
            .WithPlainNumberFormat()
            .WithLargeComment("Cette valeur est calculée en calculant d'une part les cotisations sur une assiette approximative puis, après calcul de la CSG et la CRDS, de la comparaison entre cette première assiette et une deuxième qui est la somme entre le revenu net, la CSG non déductible et la CRDS. En cas d'écart, on modifie la première assiette pour se rapprocher de la seconde et on itère jusqu'à ce que la différence entre les deux soit inférieure à 1 €.");

        worksheet.Cell("A5").Value = "PASS";
        worksheet.Cell("B5")
            .SetValue(_pass.Valeur)
            .WithShortComment($"Plafond Annuel de la Sécurité Sociale pour l'année {_calculateur.Annee}");

        worksheet.Cell("A6").Value = "Plafond retraite complémentaire";
        worksheet.Cell("B6").Value = _constantesAvecHistorique.PlafondsRetraiteComplementaireArtisansCommercants;

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

        workbook.SaveAs(output);
    }

    private (string celluleCotisationsMaladie, string celluleIndemnites) ExporteLesCotisationsMaladie(IXLWorksheet worksheet)
    {
        worksheet.Cell("A8").Value = "Cotisations maladie hors indemnités";
        const string celluleCotisationsMaladie = "B8";
        worksheet.Cell(celluleCotisationsMaladie)
            .WithImportantFormula($"=B4*{_calculateur.MaladieHorsIndemnitesJournalieres.Taux}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.MaladieHorsIndemnitesJournalieres.Explication);

        worksheet.Cell("A9").Value = "Cotisations indemnités maladie";
        const string celluleIndemnites = "B9";
        worksheet.Cell(celluleIndemnites)
            .WithImportantFormula($"=B4*{_calculateur.MaladieIndemnitesJournalieres.Taux}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.MaladieIndemnitesJournalieres.Explication);

        return (celluleCotisationsMaladie, celluleIndemnites);
    }

    private (string celluleRetraiteDeBase, string celluleRetraiteComplementaire) ExporteLesCotisationsRetraite(IXLWorksheet worksheet)
    {
        worksheet.Cell("A12").Value = "Retraite de base";
        worksheet.Cell("B11").Value = "Revenus tranche 1";
        worksheet.Cell("B12").FormulaA1 = "=B5"; // TODO: implem avec des revenus < PASS
        worksheet.Cell("C11").Value = "Taux 1";
        worksheet.Cell("C12")
            .SetValue(_calculateur.RetraiteDeBase.Taux1)
            .WithPercentFormat();
        worksheet.Cell("D11").Value = "Revenus tranche 2";
        worksheet.Cell("D12")
            .SetFormulaA1("=B4-B5") // TODO: implem avec des revenus < PASS
            .WithPlainNumberFormat();
        worksheet.Cell("E11").Value = "Taux 2";
        worksheet.Cell("E12")
            .SetValue(_calculateur.RetraiteDeBase.Taux2)
            .WithPercentFormat();
        worksheet.Cell("F11").Value = "Cotisation";
        const string celluleRetraiteDeBase = "F12";
        worksheet.Cell(celluleRetraiteDeBase)
            .WithImportantFormula("=B12*C12+D12*E12")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.RetraiteDeBase.Explication);

        worksheet.Cell("A13").Value = "Retraite complémentaire";
        worksheet.Cell("B13").FormulaA1 = "=B6"; // TODO: implem avec des revenus < PASS
        worksheet.Cell("C13")
            .SetValue(_calculateur.RetraiteComplementaire.Taux1)
            .WithPercentFormat();
        worksheet.Cell("D13")
            .SetFormulaA1("=B4-B6") // TODO: implem avec des revenus < PASS
            .WithPlainNumberFormat();
        worksheet.Cell("E13")
            .SetValue(_calculateur.RetraiteComplementaire.Taux2)
            .WithPercentFormat();
        const string celluleRetraiteComplementaire = "F13";
        worksheet.Cell(celluleRetraiteComplementaire)
            .WithImportantFormula("=B13*C13+D13*E13")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.RetraiteComplementaire.Explication);

        return (celluleRetraiteDeBase, celluleRetraiteComplementaire);
    }

    private string ExporteLesCotisationsInvaliditeDeces(IXLWorksheet worksheet)
    {
        worksheet.Cell("A15").Value = "Cotisations invalidité/décès";
        const string celluleResultat = "B15";
        worksheet.Cell(celluleResultat)
            .WithImportantFormula($"=MIN(B4,B5)*{_calculateur.InvaliditeDeces.Taux}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.InvaliditeDeces.Explication);

        return celluleResultat;
    }

    private string ExporteLesCotisationsAllocationsFamiliales(IXLWorksheet worksheet)
    {
        worksheet.Cell("A16").Value = "Cotisations allocations familiales";
        const string celluleResultat = "B16";
        worksheet.Cell(celluleResultat)
            .WithImportantFormula($"=B4*{_calculateur.AllocationsFamiliales.Taux}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.AllocationsFamiliales.Explication);

        return celluleResultat;
    }

    private string ExporteLesCotisationsFormationProfessionnelle(IXLWorksheet worksheet)
    {
        worksheet.Cell("A20").Value = "Formation professionnelle";
        const string celluleResultat = "B20";
        worksheet.Cell(celluleResultat)
            .WithImportantFormula($"=B5*{_calculateur.FormationProfessionnelle.Taux}")
            .WithPlainNumberFormat()
            .WithShortComment(_calculateur.FormationProfessionnelle.Explication);

        return celluleResultat;
    }

    private (string celluleCsgDeductible, string celluleCsgNonDeductible, string celluleCrds) ExporteCsgEtCrds(IXLWorksheet worksheet)
    {
        worksheet.Cell("A22").Value = "Revenus pris en compte pour CSG/CRDS";
        worksheet.Cell("B22")
            .SetFormulaA1("=B4+B18")
            .WithPlainNumberFormat();

        worksheet.Cell("A24").Value = "CSG déductible";
        const string celluleCsgDeductible = "B24";
        worksheet.Cell(celluleCsgDeductible)
            .WithImportantFormula($"=B22*{_calculateur.CSGDeductible.Taux}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.CSGDeductible.Explication);

        worksheet.Cell("A25").Value = "CSG non déductible";
        const string celluleCsgNonDeductible = "B25";
        worksheet.Cell(celluleCsgNonDeductible)
            .WithImportantFormula($"=B22*{_calculateur.CSGNonDeductible.Taux}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.CSGNonDeductible.Explication);

        worksheet.Cell("A26").Value = "CRDS";
        const string celluleCrds = "B26";
        worksheet.Cell(celluleCrds)
            .WithImportantFormula($"=B22*{_calculateur.CRDS.Taux}")
            .WithPlainNumberFormat()
            .WithLargeComment(_calculateur.CRDS.Explication);

        return (celluleCsgDeductible, celluleCsgNonDeductible, celluleCrds);
    }
}