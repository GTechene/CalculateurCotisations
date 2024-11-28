function exportExcel() {
  return {
    isExporting: false,
    getClass() {
      if (this.isExporting === true) {
        return "btn-exporting";
      } else {
        return "btn-export";
      }
    },
    exportData(revenuNet, annee, cotisationsFacultatives) {
      this.isExporting = true;

      const url = `/cotisations/export/${revenuNet}?annee=${annee}&cotisationsFacultatives=${cotisationsFacultatives}`;
      fetch(url)
        .then((response) => response.blob())
        .then((blob) => {
          const url = window.URL.createObjectURL(blob);
          const a = document.createElement("a");
          a.href = url;
          a.download = `Export cotisations ${annee}.xlsx`;
          document.body.appendChild(a);
          a.click();
          window.URL.revokeObjectURL(url);
          this.isExporting = false;
        });
    },
  };
}
