document.addEventListener('DOMContentLoaded', function () {
    const inputTitulo = document.getElementById('filtroTitulo');
    const selectEstado = document.getElementById('filtroEstado');
    const tarjetas = document.querySelectorAll('#listadoPreguntas .card');

    function filtrar() {
        const texto = inputTitulo.value.toLowerCase();
        const estado = selectEstado.value;

        tarjetas.forEach(card => {
            const titulo = card.getAttribute('data-titulo');
            const estadoCard = card.getAttribute('data-estado');

            const coincideTitulo = titulo.includes(texto);
            const coincideEstado = !estado || estadoCard === estado;

            if (coincideTitulo && coincideEstado) {
                card.classList.remove('d-none');
            } else {
                card.classList.add('d-none');
            }
        });
    }

    inputTitulo.addEventListener('input', filtrar);
    selectEstado.addEventListener('change', filtrar);
});