function FormulaireCotisations() {
    return {
        formData: {
            revenuNet: 0,
            annee: 2024,
            cotisationsFacultatives: 0,
        },
        cotisationsFacultativesValidees: 0,
        formWasSubmitted: false,
        cotisationsDetaillees: {
            maladieHorsIndemnitesJournalieres: { valeur: 0, explication: 'MICHEL' },
            maladieIndemnitesJournalieres: { valeur: 0, explication: '' },
            retraiteDeBase: { valeur: 0, explication: '' },
            retraiteComplementaire: { valeur: 0, explication: '' },
            invaliditeDeces: { valeur: 0, explication: '' },
            allocationsFamiliales: { valeur: 0, explication: '' },
            totalCotisationsObligatoires: 0,
            csgDeductible: { valeur: 0, explication: '' },
            csgNonDeductible: { valeur: 0, explication: '' },
            crds: { valeur: 0, explication: '' },
            formationProfessionnelle: { valeur: 0, explication: '' },
            grandTotal: 0
        },
        submitForm() {
            const formData = this.formData;
            const url = `/cotisations/v2/precises/${formData.revenuNet}?annee=${formData.annee}&cotisationsFacultatives=${formData.cotisationsFacultatives}`;
            fetch(url)
                .then(response => response = response.json())
                .then(data => {
                    this.cotisationsDetaillees = data;
                    this.formWasSubmitted = true;
                    this.cotisationsFacultativesValidees = formData.cotisationsFacultatives;
                })
                .then(() => {
                    document.querySelectorAll('td[class="explication-cell"] a').forEach(a => {
                        const popover = bootstrap.Popover.getInstance(a)
                        popover.setContent({
                            '.popover-body': a.dataset.bsContent
                        })
                    })
                })
                .catch(error => {
                    console.error('Error fetching data:', error);
                });;
        },
    };
}