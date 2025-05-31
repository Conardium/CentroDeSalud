document.addEventListener("DOMContentLoaded", function () {
    const btnNuevaFranja = document.getElementById("btnNuevaFranja");
    const btnAnadir = document.getElementById("btnAnadir");

    const tablaBody = document.getElementById("tabla-franjas-body");
    const tablaContainer = tablaBody.closest("table").parentElement;

    const formularioFranja = document.getElementById("formularioFranja");
    const diaSemana = document.getElementById("diaSemana");
    const horaInicio = document.getElementById("horaInicio");
    const horaFin = document.getElementById("horaFin");

    let contadorFranjas = 0;

    // Ocultar tabla y formulario al iniciar
    tablaContainer.style.display = "none";
    ocultarFormulario();

    // Mostrar formulario al hacer clic en “Nueva franja”
    btnNuevaFranja.addEventListener("click", function (e) {
        e.preventDefault();
        mostrarFormulario();
    });

    // Añadir franja
    btnAnadir.addEventListener("click", function (e) {
        e.preventDefault();

        if (!diaSemana.value || !horaInicio.value || !horaFin.value || diaSemana.value == 0) {
            alert("Por favor, completa todos los campos.");
            return;
        }

        if (horaInicio.value >= horaFin.value) {
            alert("Seleccione una hora correcta");
            return;
        }

        // Validar que el día no esté ya deshabilitado
        if (diaSemana.options[diaSemana.selectedIndex].disabled) {
            alert("Este día ya ha sido añadido. Elimina la franja correspondiente para volver a usarlo.");
            return;
        }

        const diaTexto = diaSemana.options[diaSemana.selectedIndex].text;
        const horaInicioVal = horaInicio.value;
        const horaFinVal = horaFin.value;
        const diaValor = diaSemana.value;

        // Crear fila de tabla con botón de eliminar
        const fila = document.createElement("tr");
        fila.setAttribute("data-index", contadorFranjas);
        fila.setAttribute("data-dia", diaValor);
        fila.innerHTML = `
            <td>${diaTexto}</td>
            <td>${horaInicioVal}</td>
            <td>${horaFinVal}</td>
            <td><button type="button" class="btn btn-danger btn-sm eliminar-franja"><i class="fa-regular fa-trash-can"></i> Eliminar</button></td>
        `;
        tablaBody.appendChild(fila);

        // Mostrar tabla si no estaba visible
        tablaContainer.style.display = "block";

        // Crear inputs ocultos que se servirán para enviarse al backend
        const container = document.createElement("div");
        container.setAttribute("data-index", contadorFranjas);
        container.innerHTML = `
            <input type="hidden" name="HorarioConsultas[${contadorFranjas}].DiaSemana" value="${diaValor}" />
            <input type="hidden" name="HorarioConsultas[${contadorFranjas}].HoraInicio" value="${horaInicioVal}" />
            <input type="hidden" name="HorarioConsultas[${contadorFranjas}].HoraFin" value="${horaFinVal}" />
        `;
        document.getElementById("contenedorInputsOcultos").appendChild(container);

        // Deshabilitar el día en el <select> para que no se repita
        diaSemana.options[diaSemana.selectedIndex].disabled = true;

        contadorFranjas++;

        // Limpiar campos
        diaSemana.selectedIndex = 0;
        horaInicio.value = "";
        horaFin.value = "";
        ocultarFormulario();
    });

    // Evento de eliminación de filas de la tabla
    tablaBody.addEventListener("click", function (e) {
        if (e.target && e.target.classList.contains("eliminar-franja")) {
            const fila = e.target.closest("tr");
            const index = fila.getAttribute("data-index");
            const diaValor = fila.getAttribute("data-dia");

            // Eliminar fila de tabla
            fila.remove();

            // Eliminar inputs ocultos
            const hiddenContainer = document.querySelector(`div[data-index='${index}']`);
            if (hiddenContainer) hiddenContainer.remove();

            // Habilitar de nuevo ese día en el <select>
            const option = diaSemana.querySelector(`option[value="${diaValor}"]`);
            if (option) option.disabled = false;

            // Si no hay filas, ocultar tabla
            if (tablaBody.rows.length === 0) {
                tablaContainer.style.display = "none";
            }
        }
    });

    function mostrarFormulario() {
        formularioFranja.classList.add("d-flex");
        formularioFranja.style.display = "flex";
    }

    function ocultarFormulario() {
        formularioFranja.classList.remove("d-flex");
        formularioFranja.style.display = "none";
    }
});