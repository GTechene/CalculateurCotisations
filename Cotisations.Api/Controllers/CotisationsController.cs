using System.ComponentModel.DataAnnotations;
using Cotisations.Excel;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Cotisations.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class CotisationsController : ControllerBase
{
    private const decimal RevenuMaximal = 5_000_000;

    [HttpGet("v2/precises/{revenuNet}")]
    [SwaggerOperation("Calcule les cotisations en convergeant à 1 € près pour la CSG/CRDS", "Cette méthode calcule les cotisations en faisant converger les 2 assiettes (estimée et calculée) par dichotomie jusqu'à l'euro près.")]
    public ActionResult<ResultatPrecisDeCotisationsAvecExplications> CalculeAvecConvergenceV2(
        [FromRoute][SwaggerParameter("Revenu net effectivement perçu en euros, avant impôt.", Required = true)][Range(1, (double)RevenuMaximal, ErrorMessage = "Merci de renseigner un revenu de valeur positive et inférieure à 5 000 000 €", MaximumIsExclusive = true)] decimal revenuNet,
        [FromQuery][SwaggerParameter("Année pour laquelle calculer les cotisations correspondant au revenu spécifié.", Required = false)][Range(2023, 2025)] int? annee,
        [FromQuery][SwaggerParameter("Montant des cotisations facultatives (Madelin, PER...) versées pendant l'année.", Required = false)][Range(0, (double)RevenuMaximal, ErrorMessage = "Merci de renseigner des cotisations facultatives de valeur positive ou nulle et inférieure à 5 000 000 €", MaximumIsExclusive = true)] decimal? cotisationsFacultatives
        )
    {
        var valeurAnnee = annee.GetValueOrDefault(DateTime.Today.Year);
        var valeurCotisationsFacultatives = cotisationsFacultatives.GetValueOrDefault();

        if (annee < 2025)
        {
            var calculateurAvecConvergence = new CalculateurAvecConvergence(revenuNet, valeurAnnee, valeurCotisationsFacultatives);
            calculateurAvecConvergence.Calcule();
            return new ResultatPrecisDeCotisationsAvecExplications(
                calculateurAvecConvergence.MaladieHorsIndemnitesJournalieres,
                calculateurAvecConvergence.MaladieIndemnitesJournalieres,
                calculateurAvecConvergence.RetraiteDeBase,
                calculateurAvecConvergence.RetraiteComplementaire,
                calculateurAvecConvergence.InvaliditeDeces,
                calculateurAvecConvergence.AllocationsFamiliales,
                calculateurAvecConvergence.TotalCotisationsObligatoires,
                calculateurAvecConvergence.CSGNonDeductible,
                calculateurAvecConvergence.CSGDeductible,
                calculateurAvecConvergence.CRDS,
                calculateurAvecConvergence.FormationProfessionnelle,
                calculateurAvecConvergence.GrandTotal,
                calculateurAvecConvergence.AssietteDeCalculDesCotisations
            );
        }

        var calculateur = Calculateurs.TrouveUnCalculateur(valeurAnnee);
        calculateur.CalculeLesCotisations(revenuNet + valeurCotisationsFacultatives);

        return Ok(new ResultatPrecisDeCotisationsAvecExplications(
                calculateur.MaladieHorsIndemnitesJournalieres,
                calculateur.MaladieIndemnitesJournalieres,
                calculateur.RetraiteDeBase,
                calculateur.RetraiteComplementaire,
                calculateur.InvaliditeDeces,
                calculateur.AllocationsFamiliales,
                calculateur.TotalCotisationsObligatoires,
                calculateur.CSGNonDeductible,
                calculateur.CSGDeductible,
                calculateur.CRDSNonDeductible,
                calculateur.FormationProfessionnelle,
                calculateur.GrandTotal,
                calculateur.AssietteDeCalculDesCotisations
            ));
    }

    [HttpGet("export/{revenuNet}")]
    public IActionResult TelechargeFichierExcel(
        [FromRoute][SwaggerParameter("Revenu net effectivement perçu en euros, avant impôt.", Required = true)][Range(1, (double)RevenuMaximal, ErrorMessage = "Merci de renseigner un revenu de valeur positive et inférieure à 5 000 000 €", MaximumIsExclusive = true)] decimal revenuNet,
        [FromQuery][SwaggerParameter("Année pour laquelle calculer les cotisations correspondant au revenu spécifié.", Required = false)][Range(2023, 2025)] int? annee,
        [FromQuery][SwaggerParameter("Montant des cotisations facultatives (Madelin, PER...) versées pendant l'année.", Required = false)][Range(0, (double)RevenuMaximal, ErrorMessage = "Merci de renseigner des cotisations facultatives de valeur positive ou nulle et inférieure à 5 000 000 €", MaximumIsExclusive = true)] decimal? cotisationsFacultatives
    )
    {
        var valeurAnnee = annee.GetValueOrDefault(DateTime.Today.Year);
        var valeurCotisationsFacultatives = cotisationsFacultatives.GetValueOrDefault();

        ICalculateur calculateur;

        if (annee < 2025)
        {
            var calculateurAvecConvergence = new CalculateurAvecConvergence(revenuNet, valeurAnnee, valeurCotisationsFacultatives);
            calculateurAvecConvergence.Calcule();
            calculateur = calculateurAvecConvergence.Calculateur;
        }
        else
        {
            calculateur = Calculateurs.TrouveUnCalculateur(valeurAnnee);
            calculateur.CalculeLesCotisations(revenuNet);
        }

        var exporteur = new ExporteurExcel(calculateur, valeurAnnee, revenuNet, valeurCotisationsFacultatives);

        var stream = new MemoryStream();
        exporteur.Exporte(stream);
        if (stream.CanSeek)
        {
            stream.Seek(0, SeekOrigin.Begin);
        }

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Export cotisations {valeurAnnee}.xlsx");
    }
}