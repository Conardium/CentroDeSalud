document.addEventListener('DOMContentLoaded', function () {
    const inputNombre = document.getElementById('filtroNombre');
    const selectEspecialidad = document.getElementById('filtroEspecialidad');
    const cards = document.querySelectorAll('.medico-card');

    function normalizarTexto(texto) {
        return texto.toLowerCase().normalize("NFD").replace(/[\u0300-\u036f]/g, "");
    }

    function filtrar() {
        const nombreFiltro = normalizarTexto(inputNombre.value.trim());
        const especialidadFiltro = normalizarTexto(selectEspecialidad.value);

        cards.forEach(card => {
            const nombre = normalizarTexto(card.querySelector('.nombre-medico').textContent);
            const especialidad = normalizarTexto(card.querySelector('.especialidad-medico').textContent).replace(/\s+/g, '');

            const coincideNombre = nombre.includes(nombreFiltro);
            const coincideEspecialidad = especialidadFiltro === '' || especialidad.includes(especialidadFiltro);

            if (coincideNombre && coincideEspecialidad) {
                card.style.display = 'block';
            } else {
                card.style.display = 'none';
            }
        });
    }

    inputNombre.addEventListener('input', filtrar);
    selectEspecialidad.addEventListener('change', filtrar);

    //Limpiamos el input del filtro al hacer click
    inputNombre.addEventListener('click', function () {
        inputNombre.value = '';
        filtrar();
    });
});