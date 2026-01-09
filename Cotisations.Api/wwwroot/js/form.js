document.addEventListener('alpine:init', () => {
  Alpine.data('gestionDuFormulaire', () => ({
    formData: {
      revenuNet: 0,
      annee: 2025,
      cotisationsFacultatives: 0,
    },
    anneeValidee: 2025,
    cotisationsFacultativesValidees: 0,
    formulaireEnvoye: false,
    formulaireEnvoyeAuMoinsUneFois: false,
    cotisationsDetaillees: {
      maladieHorsIndemnitesJournalieres: { valeur: 0, explication: "" },
      maladieIndemnitesJournalieres: { valeur: 0, explication: "" },
      retraiteDeBase: { valeur: 0, explication: "" },
      retraiteComplementaire: { valeur: 0, explication: "" },
      invaliditeDeces: { valeur: 0, explication: "" },
      allocationsFamiliales: { valeur: 0, explication: "" },
      totalCotisationsObligatoires: 0,
      csgDeductible: { valeur: 0, explication: "" },
      csgNonDeductible: { valeur: 0, explication: "" },
      crds: { valeur: 0, explication: "" },
      formationProfessionnelle: { valeur: 0, explication: "" },
      grandTotal: 0,
    },

    init() {
      const urlParams = new URLSearchParams(window.location.search);
      
      if (urlParams.has('revenuNet')) {
        this.formData.revenuNet = urlParams.get('revenuNet');
      }      
      
      if (urlParams.has('annee')) {
        this.formData.annee = urlParams.get('annee');
      }

      if (urlParams.has('cotisationsFacultatives')) {
        this.formData.cotisationsFacultatives = urlParams.get('cotisationsFacultatives');
      }
            
      if (urlParams.has('revenuNet')) {
        // nextTick permet d'attendre que le composant soit rendu dans le navigateur avant de soumettre le formulaire
        Alpine.nextTick(() => {
          this.submitForm();
        });
      }
    },

    submitForm() {
      const formData = this.formData;

      modifyUrl();

      const serverUrl = `/cotisations/v2/precises/${formData.revenuNet}?annee=${formData.annee}&cotisationsFacultatives=${formData.cotisationsFacultatives}`;

      fetch(serverUrl)
        .then((response) => (response = response.json()))
        .then((data) => {
          this.cotisationsDetaillees = data;
          this.anneeValidee = formData.annee;
          this.formulaireEnvoye = true;
          this.formulaireEnvoyeAuMoinsUneFois = true;
          this.cotisationsFacultativesValidees = formData.cotisationsFacultatives;
        })
        .then(() => {
          document.querySelectorAll('td[class="explication-cell"] a').forEach((a) => {
            const popover = bootstrap.Popover.getInstance(a);
            popover.setContent({
              ".popover-body": a.dataset.bsContent,
            });
          });
        })
        .catch((error) => {
          console.error("Error fetching data:", error);
        });

      function modifyUrl() {
        const url = new URL(window.location);

        if (formData.revenuNet > 0)
          url.searchParams.set('revenuNet', formData.revenuNet);
        else if (url.searchParams.has('revenuNet'))
          url.searchParams.delete('revenuNet');

        url.searchParams.set('annee', formData.annee);

        if (formData.cotisationsFacultatives > 0)
          url.searchParams.set('cotisationsFacultatives', formData.cotisationsFacultatives);
        else if (url.searchParams.has('cotisationsFacultatives'))
          url.searchParams.delete('cotisationsFacultatives');

        // On change l'URL dans la barre du navigateur sans forcer un rechargement de la page
        history.pushState(null, '', url.toString());
      }
    },
  }));
});