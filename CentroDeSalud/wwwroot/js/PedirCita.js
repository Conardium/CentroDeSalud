const selectMedico = document.getElementById("MedicoId");
const fechaInput = document.getElementById("Fecha");

selectMedico.onchange = function () {
    const medicoId = selectMedico.value;
    if (medicoId !== "00000000-0000-0000-0000-000000000000") {
        $("#Fecha").trigger("change");
        $("#Fecha").prop("readonly", false);
    } else {
        desactivarDetallesCita();
        $("#Fecha").prop("readonly", true);
    }
};

//En caso de fallar la peticion de Fetch quitamos los datos adicionales del formulario
function desactivarDetallesCita() {
    debugger;
    document.getElementById('divHora').style.display = 'none';
    document.getElementById('divMotivo').style.display = 'none';
    document.getElementById('divDetalles').style.display = 'none';
    document.getElementById('BotonPedirCita').disabled = true;

}

function activarDetallesCita() {
    document.getElementById('divHora').style.display = 'block';
    document.getElementById('divMotivo').style.display = 'block';
    document.getElementById('divDetalles').style.display = 'block';
    document.getElementById('BotonPedirCita').disabled = false;

}