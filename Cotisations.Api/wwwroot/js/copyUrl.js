Alpine.data('urlCopier', () => ({
  copyUrl: false,

  copyCurrentUrl() {
    if (navigator.clipboard && navigator.clipboard.writeText) {
      navigator.clipboard.writeText(window.location.href)
        .then(() => {
          this.copyUrl = true;
          setTimeout(() => {
            this.copyUrl = false;
          }, 2000);
        })
        .catch((err) => {
          console.error('Failed to copy URL: ', err);
        });
    }
  }
}));