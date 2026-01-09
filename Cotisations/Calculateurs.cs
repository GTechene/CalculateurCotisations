namespace Cotisations;

public class Calculateurs
{
    public static ICalculateur TrouveUnCalculateur(int annee)
    {
        switch (annee)
        {
            case 2023:
                return new Calculateur2023();
            case 2024:
                return new Calculateur2024();
            case 2025:
                return new Calculateur2025();
            case 2026:
                return new Calculateur2026();
            default:
                throw new IndexOutOfRangeException("Ce simulateur ne gère que des années comprises entre 2023 et 2026 incluses.");
        }
    }
}