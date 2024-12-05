using NFluent;

namespace Cotisations.Tests.Unit;

internal class Calculateur2025Should
{
    // TODO: REFACTO pour que CalculeLesCotisations renvoie un objet de type Cotisations ? Voir si ça passe avec le CalculateurAvecConvergence (y a pas de raison)
    // TODO: REFACTO pour avoir un CalculateurCommun qui soit un objet utilisé par composition dans les Calculateurs202* plutôt que de l'héritage

    [Test]
    public void Utiliser_une_assiette_egale_a_74_pct_du_revenu_net_quand_le_revenu_net_est_entre_3188_et_235500_euros()
    {
        var calculateur = new Calculateur2025();

        const decimal revenu = 50_000m;
        calculateur.CalculeLesCotisations(revenu);

        Check.That(calculateur.Assiette).IsEqualTo(0.74m * revenu);
    }

    [Test]
    public void Utiliser_une_assiette_avec_abattement_plancher_quand_le_revenu_net_est_inferieur_a_3188_euros()
    {
        var calculateur = new Calculateur2025();

        const decimal revenu = 3000m;
        calculateur.CalculeLesCotisations(revenu);

        // C'est 1.76% du PASS, 1.76% de 47 100 en 2025
        const decimal abattementPlancher = 828.96m;
        const decimal assietteAttendue = revenu - abattementPlancher;
        Check.That(calculateur.Assiette).IsEqualTo(assietteAttendue);
    }

    [Test]
    public void Utiliser_une_assiette_avec_abattement_plafond_quand_le_revenu_net_est_superieur_a_235500_euros()
    {
        var calculateur = new Calculateur2025();

        const decimal revenu = 250_000m;
        calculateur.CalculeLesCotisations(revenu);

        // C'est 130% du PASS, 130% de 47 100 en 2025
        const decimal abattementPlafond = 61_230m;
        const decimal assietteAttendue = revenu - abattementPlafond;
        Check.That(calculateur.Assiette).IsEqualTo(assietteAttendue);
    }
}
