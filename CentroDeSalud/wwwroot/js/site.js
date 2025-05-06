
function mostrarAlerta(mensaje) {
    const contenedor = document.getElementById("contenedorAlertas");

    //Limpiamos el contenido del contenedor
    contenedor.innerHTML = '';

    const alerta = document.createElement("div");
    alerta.className = "alert alert-warning alert-dismissible fade show";
    alerta.role = "alert";
    alerta.innerHTML = `
        <i class="fa-solid fa-triangle-exclamation"></i> ${mensaje}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>`;

    contenedor.appendChild(alerta);
}