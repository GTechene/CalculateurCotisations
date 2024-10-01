namespace Cotisations;

public class Calculateurs
{
    public static CalculateurDeBase TrouveUnCalculateur(int annee)
    {
        switch (annee)
        {
            case 2023:
                return new Calculateur2023();
            default:
                return new Calculateur2024();
        }
    }
}