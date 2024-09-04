namespace CalculCotisations;

public class Calculateur2024 : CalculateurDeBase
{
    private readonly decimal _cotisationsIndemnitesMaladie;

    public Calculateur2024() : base(new Constantes2024(), new PlafondAnnuelSecuriteSociale(2024))
    {
        _cotisationsIndemnitesMaladie = ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass;
    }

    protected override void CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette)
    {
        if (assiette < PASS.Valeur40Pct)
        {
            var cotisations = _cotisationsIndemnitesMaladie * PASS.Valeur40Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecExplication(cotisations, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS), donc le taux fixe de {_cotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plancher de 40% du PASS, soit {cotisations:C0} de cotisations.");
            return;
        }

        if (assiette < PASS.Valeur500Pct)
        {
            var cotisations = _cotisationsIndemnitesMaladie * assiette;
            MaladieIndemnitesJournalieres = new ResultatAvecExplication(cotisations, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {_cotisationsIndemnitesMaladie * 100:F1}% est appliqué à cette assiette, soit {cotisations:C0} de cotisations.");
        }
        else
        {
            var cotisations = _cotisationsIndemnitesMaladie * PASS.Valeur500Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {_cotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plafond de 500% du PASS, soit {cotisations:C0} de cotisations.");
        }
    }

    protected override void CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(decimal assiette)
    {
        if (assiette <= ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants)
        {
            var valeur = assiette * Taux.RetraiteComplementairePremiereTrancheArtisansCommercants;
            RetraiteComplementaire = new ResultatAvecExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {Taux.RetraiteComplementairePremiereTrancheArtisansCommercants * 100:N0}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.");
        }
        else
        {
            var cotisationPremiereTranche = Taux.RetraiteComplementairePremiereTrancheArtisansCommercants * ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants;
            var cotisationDeuxiemeTranche = Taux.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * (Math.Min(assiette, PASS.Valeur400Pct) - ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants);

            var cotisations = cotisationPremiereTranche + cotisationDeuxiemeTranche;
            RetraiteComplementaire = new ResultatAvecExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {Taux.RetraiteComplementairePremiereTrancheArtisansCommercants * 100:N0}% est appliqué à la part des revenus inférieure à cette valeur et le taux fixe de {Taux.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * 100:N0}% est appliqué à la part des revenus qui y est supérieure, soit {cotisations:C0} de cotisations.");
        }
    }
}