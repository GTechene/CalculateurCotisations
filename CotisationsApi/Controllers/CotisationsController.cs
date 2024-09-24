using CalculCotisations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CotisationsApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CotisationsController : ControllerBase
{
    [Obsolete("Utilisez la v2 svp.")]
    [HttpGet("precises/{revenuNet}")]
    [SwaggerOperation("Calcule les cotisations en convergeant à 1 € près pour la CSG/CRDS", "Cette méthode calcule les cotisations en faisant converger les 2 assiettes (estimée et calculée) par dichotomie jusqu'à l'euro près.")]
    public ResultatPrecisDeCotisations CalculeAvecConvergence([FromRoute][SwaggerParameter("Revenu net effectivement perçu en euros, avant impôt.")] decimal revenuNet)
    {
        var calculateur = new CalculateurAvecConvergence(revenuNet);
        calculateur.Calcule();

        return new ResultatPrecisDeCotisations(
            calculateur.MaladieHorsIndemnitesJournalieres.Valeur,
            calculateur.MaladieIndemnitesJournalieres.Valeur,
            calculateur.RetraiteDeBase.Valeur,
            calculateur.RetraiteComplementaire.Valeur,
            calculateur.InvaliditeDeces.Valeur,
            calculateur.AllocationsFamiliales.Valeur,
            calculateur.TotalCotisationsObligatoires,
            calculateur.CSGNonDeductible.Valeur,
            calculateur.CSGDeductible.Valeur,
            calculateur.CRDS.Valeur,
            calculateur.FormationProfessionnelle.Valeur,
            calculateur.GrandTotal
            );
    }

    [HttpGet("v2/precises/{revenuNet}")]
    // TODO : checker que l'année est >= 2023
    [SwaggerOperation("Calcule les cotisations en convergeant à 1 € près pour la CSG/CRDS", "Cette méthode calcule les cotisations en faisant converger les 2 assiettes (estimée et calculée) par dichotomie jusqu'à l'euro près.")]
    public ResultatPrecisDeCotisationsAvecExplications CalculeAvecConvergenceV2(
        [FromRoute][SwaggerParameter("Revenu net effectivement perçu en euros, avant impôt.", Required = true)] decimal revenuNet,
        [FromQuery][SwaggerParameter("Année pour laquelle calculer les cotisations correspondant au revenu spécifié.", Required = false)] int? annee,
        [FromQuery][SwaggerParameter("Montant des cotisations facultatives (Madelin, PER...) versées pendant l'année.", Required = false)] decimal? cotisationsFacultatives
        )
    {
        var calculateur = new CalculateurAvecConvergence(revenuNet, annee.GetValueOrDefault(DateTime.Today.Year), cotisationsFacultatives.GetValueOrDefault());
        calculateur.Calcule();

        return new ResultatPrecisDeCotisationsAvecExplications(
            calculateur.MaladieHorsIndemnitesJournalieres,
            calculateur.MaladieIndemnitesJournalieres,
            calculateur.RetraiteDeBase,
            calculateur.RetraiteComplementaire,
            calculateur.InvaliditeDeces,
            calculateur.AllocationsFamiliales,
            calculateur.TotalCotisationsObligatoires,
            calculateur.CSGNonDeductible,
            calculateur.CSGDeductible,
            calculateur.CRDS,
            calculateur.FormationProfessionnelle,
            calculateur.GrandTotal
        );
    }
}