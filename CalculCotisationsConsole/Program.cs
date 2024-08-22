using CalculCotisations;

string revenuStr;

if (args.Length == 0)
{
    Console.WriteLine("Entrez votre revenu net annuel : ");
    revenuStr = Console.ReadLine() ?? string.Empty;
}
else
{
    revenuStr = args[0];
}

var parsing = decimal.TryParse(revenuStr, out var revenu);
if (!parsing)
{
    Console.WriteLine("Impossible de lire cette information sous format numérique. Avez-vous utilisé des caractères non numériques ? Exemple de données autorisées : 50000");
    return;
}


var calculateur = new CalculateurAvecConvergence(revenu);
calculateur.Calcule();

Console.WriteLine($"Cotisations maladie (total) =       {calculateur.MaladieHorsIndemnitesJournalieres + calculateur.MaladieIndemnitesJournalieres:F2}");
Console.WriteLine($"Allocations familiales =            {calculateur.AllocationsFamiliales:F2}");
Console.WriteLine($"Retraite base =                     {calculateur.RetraiteDeBase:F2}");
Console.WriteLine($"Retraite complémentaire =           {calculateur.RetraiteComplementaire:F2}");
Console.WriteLine($"Invalidité/Décès =                  {calculateur.InvaliditeDeces:F2}");
Console.WriteLine($"Total cotisations obligatoires =    {calculateur.TotalCotisationsObligatoires:F2}");
Console.WriteLine("---------------------------------------------------");
Console.WriteLine($"CSG non déductible =                {calculateur.CSGNonDeductible:F2}");
Console.WriteLine($"CSG déductible =                    {calculateur.CSGDeductible:F2}");
Console.WriteLine($"CRDS non déductible =               {calculateur.CRDS:F2}");
Console.WriteLine($"Formation professionnelle =         {calculateur.FormationProfessionnelle:F2}");
Console.WriteLine($"Total cotisations :                 {calculateur.GrandTotal:F2}");