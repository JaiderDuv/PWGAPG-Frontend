// Carrusel horizontal de recetas: flechas, scroll suave y estado de las flechas.
(function () {
    const margen = 2;

    function anchoPaso(track) {
        const tarjeta = track.querySelector('.rc-mini');
        if (!tarjeta) return Math.max(200, track.clientWidth * 0.8);

        const estilos = getComputedStyle(track);
        const gap = parseFloat(estilos.columnGap || estilos.gap || '16') || 16;
        const ancho = tarjeta.getBoundingClientRect().width;
        const porVista = Math.max(1, Math.floor((track.clientWidth + gap) / (ancho + gap)));
        return (ancho + gap) * porVista;
    }

    function actualizar(track) {
        const wrap = track.closest('.rc-row-wrap');
        if (!wrap) return;

        const maximo = track.scrollWidth - track.clientWidth;
        const alInicio = track.scrollLeft <= margen;
        const alFinal = maximo <= margen || track.scrollLeft >= maximo - margen;

        wrap.classList.toggle('no-prev', alInicio);
        wrap.classList.toggle('no-next', alFinal);
    }

    function enlazar(track) {
        if (track.dataset.carruselListo === '1') return;
        track.dataset.carruselListo = '1';

        track.addEventListener('scroll', () => actualizar(track), { passive: true });
        actualizar(track);
    }

    window.carrusel = {
        // Se llama después de cada render: enlaza las filas nuevas y recalcula flechas.
        iniciar() {
            document.querySelectorAll('.rc-row-track').forEach(enlazar);
            document.querySelectorAll('.rc-row-track').forEach(actualizar);
        },

        mover(id, direccion) {
            const track = document.getElementById(id);
            if (!track) return;

            const paso = anchoPaso(track) * (direccion < 0 ? -1 : 1);
            track.scrollBy({ left: paso, behavior: 'smooth' });
            window.setTimeout(() => actualizar(track), 400);
        }
    };

    // Al cambiar el tamaño de la ventana, las flechas deben reevaluarse.
    window.addEventListener('resize', () => {
        document.querySelectorAll('.rc-row-track').forEach(actualizar);
    });
})();
