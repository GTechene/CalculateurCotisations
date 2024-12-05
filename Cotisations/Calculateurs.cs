namespace Cotisations;

public class Calculateurs
{
    public static ICalculateur TrouveUnCalculateur(int annee)
    {
        switch (annee)
        {
            case 2023:
                return new Calculateur2023();
            case 2025:
                return new Calculateur2025();
            default:
                return new Calculateur2024();
        }
    }
}