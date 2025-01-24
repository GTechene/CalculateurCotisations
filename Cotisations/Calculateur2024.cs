namespace Cotisations;

public class Calculateur2024 : CalculateurDeBase
{
    private readonly decimal _tauxDeCotisationsIndemnitesMaladie;

    public Calculateur2024() : base(new Constantes2024(), new PlafondAnnuelSecuriteSociale(2024))
    {
        _tauxDeCotisationsIndemnitesMaladie = ConstantesHistoriques.CotisationsIndemnitesMaladiePourRevenusInferieursA40PctDuPass;
    }

    protected override void CalculeLesCotisationsPourIndemnitesMaladie(decimal assiette)
    {
        if (assiette < PASS.Valeur40Pct)
        {
            var cotisations = _tauxDeCotisationsIndemnitesMaladie * PASS.Valeur40Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est inférieure à {PASS.Valeur40Pct:C0} (40% du PASS), donc le taux fixe de {_tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plancher de 40% du PASS, soit {cotisations:C0} de cotisations.", _tauxDeCotisationsIndemnitesMaladie);
            return;
        }

        if (assiette < PASS.Valeur500Pct)
        {
            var cotisations = _tauxDeCotisationsIndemnitesMaladie * assiette;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est comprise entre {PASS.Valeur40Pct:C0} (40% du PASS) et {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {_tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à cette assiette, soit {cotisations:C0} de cotisations.", _tauxDeCotisationsIndemnitesMaladie);
        }
        else
        {
            var cotisations = _tauxDeCotisationsIndemnitesMaladie * PASS.Valeur500Pct;
            MaladieIndemnitesJournalieres = new ResultatAvecTauxUniqueEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {PASS.Valeur500Pct:C0} (500% du PASS), donc le taux fixe de {_tauxDeCotisationsIndemnitesMaladie * 100:F1}% est appliqué à ce plafond de 500% du PASS, soit {cotisations:C0} de cotisations.", _tauxDeCotisationsIndemnitesMaladie);
        }
    }

    protected override void CalculeLaRetraiteComplementaireSelonLeRegimeArtisansCommercants(decimal assiette)
    {
        if (assiette <= ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants)
        {
            var valeur = assiette * TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants;
            RetraiteComplementaire = new ResultatAvecTauxMultiplesEtExplication(valeur, $"L'assiette de {assiette:C0} est inférieure à {ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants * 100:N0}% est appliqué à cette assiette, soit {valeur:C0} de cotisations.", TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants, 0m);
        }
        else
        {
            var cotisationPremiereTranche = TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants * ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants;
            var cotisationDeuxiemeTranche = TauxInchanges.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * (Math.Min(assiette, PASS.Valeur400Pct) - ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants);

            var cotisations = cotisationPremiereTranche + cotisationDeuxiemeTranche;
            RetraiteComplementaire = new ResultatAvecTauxMultiplesEtExplication(cotisations, $"L'assiette de {assiette:C0} est supérieure à {ConstantesHistoriques.PlafondsRetraiteComplementaireArtisansCommercants:C0}, donc le taux fixe de {TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants * 100:N0}% est appliqué à la part des revenus inférieure à cette valeur et le taux fixe de {TauxInchanges.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants * 100:N0}% est appliqué à la part des revenus qui y est supérieure, soit {cotisations:C0} de cotisations.", TauxInchanges.RetraiteComplementairePremiereTrancheArtisansCommercants, TauxInchanges.RetraiteComplementaireDeuxiemeTrancheArtisansCommercants);
        }
    }
}