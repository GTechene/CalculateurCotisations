﻿using CalculCotisations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace CotisationsApi.Controllers;

[ApiController]
[Route("[controller]")]
public class CotisationsController : ControllerBase
{
    [HttpGet("precises/{revenuNet}")]
    [SwaggerOperation("Calcule les cotisations en convergeant à 1 € près pour la CSG/CRDS", "Cette méthode calcule les cotisations en faisant converger les 2 assiettes (estimée et calculée) par dichotomie jusqu'à l'euro près.")]
    public ResultatPrecisDeCotisations CalculeAvecConvergence([FromRoute][SwaggerParameter("Revenu net effectivement perçu en euros, avant impôt.")] decimal revenuNet)
    {
        var calculateur = new CalculateurAvecConvergence(revenuNet);
        calculateur.Calcule();

        return new ResultatPrecisDeCotisations(
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