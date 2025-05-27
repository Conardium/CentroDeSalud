document.addEventListener("DOMContentLoaded", function () {
    const btnMostrar = document.getElementById("btnMostrarFormulario");
    const preguntaBox = document.getElementById("preguntaBox");
    const formularioBox = document.getElementById("formularioBox");

    const btnCancelar = document.getElementById("btnCancelarPregunta");

    btnMostrar.addEventListener("click", function () {
        preguntaBox.classList.add("d-none");
        formularioBox.classList.remove("d-none");
    });

    btnCancelar.addEventListener("click", function () {
        formularioBox.classList.add("d-none");
        preguntaBox.classList.remove("d-none");
    });
});

document.querySelectorAll(".ver-respuestas").forEach(boton => {
    boton.addEventListener("click", function () {
        const id = this.getAttribute("data-id");
        const respuestas = document.querySelector(`.respuestas[data-id="${id}"]`);
        const total = this.getAttribute("data-respuestas");

        // Ocultar otras respuestas y restablecer texto de botones
        document.querySelectorAll(".respuestas").forEach(div => {
            if (div.getAttribute("data-id") !== id) {
                div.classList.add("d-none");

                const otherButton = document.querySelector(`.ver-respuestas[data-id="${div.getAttribute("data-id")}"]`);
                if (otherButton) {
                    const count = otherButton.getAttribute("data-respuestas");
                    otherButton.textContent = `Ver respuestas (${count})`;
                }
            }
        });

        // Alternar visibilidad de la respuesta seleccionada
        respuestas.classList.toggle("d-none");

        // Cambiar el texto del botón actual
        this.textContent = respuestas.classList.contains("d-none")
            ? `Ver respuestas (${total})`
            : `Ocultar respuestas`;
    });
});