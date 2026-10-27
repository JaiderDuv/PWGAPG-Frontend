// Las flechas de los campos numéricos siempre suman o restan 1 unidad,
// incluso cuando el valor actual tiene decimales.
(() => {
    document.addEventListener('keydown', (event) => {
        if (event.altKey || event.ctrlKey || event.metaKey || event.shiftKey) {
            return;
        }

        if (event.key !== 'ArrowUp' && event.key !== 'ArrowDown') {
            return;
        }

        const input = event.target;
        if (!(input instanceof HTMLInputElement) || input.type !== 'number') {
            return;
        }

        event.preventDefault();

        const incremento = event.key === 'ArrowUp' ? 1 : -1;
        const valorActual = Number.parseFloat(input.value);
        let siguiente = (Number.isFinite(valorActual) ? valorActual : 0) + incremento;

        if (input.min !== '') {
            const minimo = Number.parseFloat(input.min);
            if (Number.isFinite(minimo) && siguiente < minimo) {
                siguiente = minimo;
            }
        }

        if (input.max !== '') {
            const maximo = Number.parseFloat(input.max);
            if (Number.isFinite(maximo) && siguiente > maximo) {
                siguiente = maximo;
            }
        }

        // Los campos aceptan hasta tres decimales; se evitan residuos de coma flotante.
        siguiente = Math.round(siguiente * 1000) / 1000;
        input.value = String(siguiente);
        input.dispatchEvent(new Event('input', { bubbles: true }));
        input.dispatchEvent(new Event('change', { bubbles: true }));
    }, true);
})();
