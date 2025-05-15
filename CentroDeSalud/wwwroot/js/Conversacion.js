document.addEventListener('input', function (event) {
    if (event.target.classList.contains('auto-grow')) {
        event.target.style.height = 'auto';
        event.target.style.height = (event.target.scrollHeight) + 'px';
    }
});

//Funcion que se carga nada más cargar el script
(function () {
    const contenedor = document.getElementById("contenedorMensajes");
    if (!contenedor) return;

    const tolerancia = 80;

    const estaCasiAbajo = () =>
        contenedor.scrollHeight - contenedor.scrollTop - contenedor.clientHeight <= tolerancia;

    const desplazarScrollAbajo = () => {
        contenedor.scrollTop = contenedor.scrollHeight;
    };

    window.addEventListener("load", desplazarScrollAbajo);

    const observer = new MutationObserver(() => {
        if (estaCasiAbajo()) {
            desplazarScrollAbajo();
        }
    });

    observer.observe(contenedor, { childList: true, subtree: true });
})();