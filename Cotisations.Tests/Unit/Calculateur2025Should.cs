using NFluent;

namespace Cotisations.Tests.Unit;

internal class Calculateur2025Should
{
    [Test]
    public void Test_avec_revenu_brut_en_entree()
    {
        var calculateur = new Calculateur2026();

        calculateur.CalculeLesCotisations(87_571m);
    }

    // TODO: REFACTO pour que CalculeLesCotisations renvoie un objet de type Cotisations ? Voir si ça passe avec le CalculateurAvecConvergence (y a pas de raison)
    // TODO: REFACTO pour avoir un CalculateurCommun qui soit un objet utilisé par composition dans les Calculateurs202* plutôt que de l'héritage
    // TODO: fuzzer

    [Test]
    public void Utiliser_une_assiette_egale_a_74_pct_du_revenu_brut_quand_le_revenu_net_est_entre_3188_et_235500_euros()
    {
        const decimal revenu = 50_000m;
        var calculateur = new CalculateurAvecConvergence(revenu, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.RevenuBrut).IsCloseTo(72939m, 1m);
        Check.That(calculateur.AssietteDeCalculDesCotisations).IsCloseTo(72939m * 0.74m, 1m);
    }

    [Test]
    public void Utiliser_une_assiette_avec_abattement_plancher_quand_le_revenu_net_est_inferieur_a_3188_euros()
    {
        const decimal revenu = 1000m;
        var calculateur = new CalculateurAvecConvergence(revenu, 2025);

        calculateur.Calcule_Depuis_2025();

        // C'est 1.76% du PASS soit 1.76% de 47 100 en 2025
        const decimal abattementPlancher = 828.96m;
        Check.That(calculateur.Abattement).IsEqualTo(abattementPlancher);
    }

    [Test]
    public void Utiliser_une_assiette_avec_abattement_plafond_quand_le_revenu_net_est_superieur_a_235500_euros()
    {
        const decimal revenu = 250_000m;
        var calculateur = new CalculateurAvecConvergence(revenu, 2025);

        calculateur.Calcule_Depuis_2025();

        // C'est 130% du PASS soit 130% de 47 100 en 2025
        const decimal abattementPlafond = 61_230m;
        Check.That(calculateur.Abattement).IsEqualTo(abattementPlafond);
    }

    [Test]
    public void Afficher_1_chiffre_apres_la_virgule_dans_l_explication_de_la_retraite_complementaire()
    {
        const decimal revenu = 50_000m;
        var calculateur = new CalculateurAvecConvergence(revenu, 2025);

        calculateur.Calcule_Depuis_2025();

        Check.That(calculateur.RetraiteComplementaire.Explication).MatchesWildcards("*8,1%*9,1%*");

        const decimal revenuInferieurAuPlafond = 40_000m;
        calculateur = new CalculateurAvecConvergence(revenuInferieurAuPlafond, 2025);
        calculateur.Calcule_Depuis_2025();

        // Expression régulière pour exclure le mot "9,1%"
        Check.That(calculateur.RetraiteComplementaire.Explication).Matches("^((?!9,1%).)*$");
    }
}