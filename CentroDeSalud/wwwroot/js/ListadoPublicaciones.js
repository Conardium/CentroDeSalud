document.addEventListener('DOMContentLoaded', function () {

    const inputTitulo = document.getElementById('filtroTitulo');
    const selectEstado = document.getElementById('filtroEstado');
    const checkboxDestacadas = document.getElementById('filtroDestacadas');
    const cards = document.querySelectorAll('.col');

    function filtrar() {
        const input = inputTitulo.value.toLowerCase();
        const estadoSeleccionado = selectEstado.value;
        const soloDestacadas = checkboxDestacadas.checked;

        cards.forEach(card => {
            const titulo = card.dataset.titulo.toLowerCase();
            const estado = card.dataset.estado;
            const esDestacada = card.dataset.destacada === 'true';

            const coincideTitulo = titulo.includes(input);
            const coincideEstado = !estadoSeleccionado || estado === estadoSeleccionado;
            const coincideDestacada = !soloDestacadas || esDestacada;

            if (coincideTitulo && coincideEstado && coincideDestacada) {
                card.style.display = '';
            } else {
                card.style.display = 'none';
            }
        });
    }

    inputTitulo.addEventListener('input', filtrar);
    selectEstado.addEventListener('change', filtrar);
    checkboxDestacadas.addEventListener('change', filtrar);
});