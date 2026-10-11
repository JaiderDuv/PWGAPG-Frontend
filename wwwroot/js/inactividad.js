window.chefMetricsInactividad = {
    ultimaActividad: Date.now(),
    limiteMs: 15 * 60 * 1000,
    dotnetRef: null,
    manejador: null,
    intervaloId: null,
    expirada: false,

    iniciar: function (dotnetRef, minutos) {
        this.detener(); // por si ya había uno corriendo
        this.dotnetRef = dotnetRef;
        this.limiteMs = (minutos || 15) * 60 * 1000;
        this.ultimaActividad = Date.now();
        this.expirada = false;

        this.manejador = () => { this.ultimaActividad = Date.now(); };
        const eventos = ['click', 'keydown', 'mousemove', 'scroll', 'touchstart'];
        eventos.forEach(ev => document.addEventListener(ev, this.manejador, true));

        this.intervaloId = setInterval(() => this.verificar(), 5000);
    },

    verificar: function () {
        if (this.expirada) return;
        const ahora = Date.now();
        if ((ahora - this.ultimaActividad) > this.limiteMs) {
            this.expirada = true;
            if (this.dotnetRef) {
                this.dotnetRef.invokeMethodAsync('SesionExpiradaPorInactividad');
            }
        }
    },

    detener: function () {
        if (this.manejador) {
            const eventos = ['click', 'keydown', 'mousemove', 'scroll', 'touchstart'];
            eventos.forEach(ev => document.removeEventListener(ev, this.manejador, true));
            this.manejador = null;
        }
        if (this.intervaloId) {
            clearInterval(this.intervaloId);
            this.intervaloId = null;
        }
        this.dotnetRef = null;
    }
};