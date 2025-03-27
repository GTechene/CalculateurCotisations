using System.IO.Compression;
using System.Text;
using Cotisations.Excel;

namespace Cotisations.Tests.Acceptance;

public class ExportExcelShould
{
    private readonly VerifySettings _verifySettings = new();

    public ExportExcelShould()
    {
        _verifySettings.UseDirectory("Snapshots");
    }

    [Test]
    public Task Creer_un_fichier_Excel_a_partir_d_un_resultat_de_calcul_avant_2025()
    {
        var revenuNet = 50_000;
        var annee = 2024;
        var calculateur = new CalculateurAvecConvergence(revenuNet, annee);
        calculateur.Calcule();
        var exporteurExcel = new ExporteurExcel(calculateur.Calculateur, annee, revenuNet, 0);

        using var stream = new MemoryStream();
        
        exporteurExcel.Exporte(stream);

        return Verify(ExtractXmlWorksheet(stream), _verifySettings);
    }

    [Test]
    public Task Creer_un_fichier_Excel_a_partir_d_un_resultat_de_calcul_avant_2025_pour_un_revenu_inferieur_au_PASS()
    {
        var revenuNet = 40_000;
        var annee = 2024;
        var calculateur = new CalculateurAvecConvergence(revenuNet, annee);
        calculateur.Calcule();
        var exporteurExcel = new ExporteurExcel(calculateur.Calculateur, annee, revenuNet, 0);

        using var stream = new MemoryStream();

        exporteurExcel.Exporte(stream);

        return Verify(ExtractXmlWorksheet(stream), _verifySettings);
    }

    [Test]
    public Task Creer_un_fichier_Excel_a_partir_d_un_resultat_de_calcul_en_2025_incluant_les_cotisations_facultatives()
    {
        const int revenuNet = 50_000;
        const int annee = 2025;
        const decimal cotisationsFacultatives = 1000m;
        var calculateur = new Calculateur2025();
        calculateur.CalculeLesCotisations(revenuNet + cotisationsFacultatives);
        var exporteurExcel = new ExporteurExcel(calculateur, annee, revenuNet, cotisationsFacultatives);

        using var stream = new MemoryStream();

        exporteurExcel.Exporte(stream);

        return Verify(ExtractXmlWorksheet(stream), _verifySettings);
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