using System.IO.Compression;
using System.Text;
using Cotisations.Excel;

namespace Cotisations.Tests.Acceptance;

public class ExportExcelShould
{
    [Test]
    public Task Creer_un_fichier_Excel_a_partir_d_un_resultat_de_calcul()
    {
        var calculateur = new CalculateurAvecConvergence(50_000, 2024);
        calculateur.Calcule();
        var exporteurExcel = new ExporteurExcel(calculateur);

        using var stream = new MemoryStream();
        
        exporteurExcel.Exporte(stream);

        return Verify(ExtractXmlWorksheet(stream));
    }

    private static string ExtractXmlWorksheet(MemoryStream stream)
    {
        var archive = new ZipArchive(stream, ZipArchiveMode.Read);
        var sheetEntry = archive.Entries.Single(entry => entry.FullName == "xl/worksheets/sheet1.xml");
        var sheetStream = sheetEntry.Open();
        var xml = new StreamReader(sheetStream, Encoding.UTF8).ReadToEnd();
        return xml;
    }
}