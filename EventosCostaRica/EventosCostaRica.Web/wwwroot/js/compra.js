// Selección de asientos en la pantalla de compra.
(function () {
    const formulario = document.getElementById('formCompra');

    if (!formulario) {
        return;
    }

    const maximo = parseInt(formulario.dataset.max, 10);
    const asientos = formulario.querySelectorAll('input[name="asientos"]');
    const resumen = document.getElementById('resumenAsientos');
    const boton = document.getElementById('btnComprar');

    function actualizar() {
        const elegidos = Array.from(asientos).filter(function (a) { return a.checked; });

        resumen.textContent = elegidos.length > 0
            ? elegidos.map(function (a) { return a.dataset.etiqueta; }).join(', ')
            : 'Ninguno';

        boton.disabled = elegidos.length === 0;

        // Al llegar al máximo, se bloquean los demás asientos libres.
        asientos.forEach(function (asiento) {
            if (!asiento.dataset.ocupado && !asiento.checked) {
                asiento.disabled = elegidos.length >= maximo;
            }
        });
    }

    asientos.forEach(function (asiento) {
        asiento.addEventListener('change', actualizar);
    });

    actualizar();
}());
