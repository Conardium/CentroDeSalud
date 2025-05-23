document.addEventListener("DOMContentLoaded", function () {
    const fechaInicio = document.getElementById("fechaInicio");
    const fechaFin = document.getElementById("fechaFin");
    const estadoFiltro = document.getElementById("estadoFiltro");
    const filas = document.querySelectorAll("tbody tr");

    function formatearFecha(fechaStr) {
        const [dia, mes, anio] = fechaStr.split(" ")[0].split("/");
        return new Date(`${anio}-${mes}-${dia}`);
    }

    function filtrar() {
        const inicio = fechaInicio.value ? new Date(fechaInicio.value) : null;
        const fin = fechaFin.value ? new Date(fechaFin.value) : null;
        const estado = estadoFiltro.value;

        filas.forEach(fila => {
            const fechaTexto = fila.cells[1].textContent.trim();
            const fechaInforme = formatearFecha(fechaTexto);

            const estadoTexto = fila.cells[fila.cells.length - 2].textContent.trim();

            let visible = true;

            if (inicio && fechaInforme < inicio) visible = false;
            if (fin && fechaInforme > fin) visible = false;
            if (estado && !estadoTexto.includes(estado)) visible = false;

            fila.style.display = visible ? "" : "none";
        });
    }

    fechaInicio.addEventListener("change", filtrar);
    fechaFin.addEventListener("change", filtrar);
    estadoFiltro.addEventListener("change", filtrar);
});