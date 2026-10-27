(function () {
    const W = 600, H = 450;   // espacio lógico del marco 4:3
    const estados = {};

    const cubrir = (s) => Math.max(W / s.img.width, H / s.img.height) * s.zoom;

    function limitar(s) {
        const k = cubrir(s);
        s.x = Math.min(0, Math.max(W - s.img.width * k, s.x));
        s.y = Math.min(0, Math.max(H - s.img.height * k, s.y));
    }

    function centrar(s) {
        const k = cubrir(s);
        s.x = (W - s.img.width * k) / 2;
        s.y = (H - s.img.height * k) / 2;
    }

    // Zoom manteniendo fijo el punto (cx, cy), como en Instagram.
    function zoomEn(s, nuevo, cx, cy) {
        nuevo = Math.min(3, Math.max(1, nuevo));
        const r = nuevo / s.zoom;
        s.x = cx - (cx - s.x) * r;
        s.y = cy - (cy - s.y) * r;
        s.zoom = nuevo;
        limitar(s);
    }

    function pintar(s, canvas, guia) {
        const f = canvas.width / W, k = cubrir(s);
        const ctx = canvas.getContext('2d');
        ctx.fillStyle = '#fff';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        ctx.drawImage(s.img, s.x * f, s.y * f, s.img.width * k * f, s.img.height * k * f);
        if (guia) {
            ctx.strokeStyle = 'rgba(255,255,255,.75)';
            ctx.lineWidth = 1;
            ctx.beginPath();
            for (let i = 1; i < 3; i++) {
                ctx.moveTo(canvas.width * i / 3, 0); ctx.lineTo(canvas.width * i / 3, canvas.height);
                ctx.moveTo(0, canvas.height * i / 3); ctx.lineTo(canvas.width, canvas.height * i / 3);
            }
            ctx.stroke();
        }
    }

    window.recetaCrop = {
        cargar(id, dataUrl) {
            return new Promise((ok, fail) => {
                const img = new Image();
                img.onload = () => {
                    const s = estados[id] || (estados[id] = {});
                    s.img = img; s.zoom = 1; centrar(s);
                    ok(true);
                };
                img.onerror = () => fail('No se pudo leer la imagen');
                img.src = dataUrl;
            });
        },

        iniciar(id, sliderId) {
            const s = estados[id];
            const canvas = document.getElementById(id);
            if (!s || !canvas) return;
            if (s.abort) s.abort.abort();
            s.abort = new AbortController();
            const opt = { signal: s.abort.signal };
            const slider = document.getElementById(sliderId);
            const punteros = new Map();
            let ultimaDist = 0;

            const escala = () => W / canvas.getBoundingClientRect().width;
            const refrescar = (guia) => {
                pintar(s, canvas, guia);
                if (slider) slider.value = s.zoom;
            };
            s.refrescar = refrescar;

            canvas.addEventListener('pointerdown', (e) => {
                canvas.setPointerCapture(e.pointerId);
                punteros.set(e.pointerId, { x: e.clientX, y: e.clientY });
                if (punteros.size === 2) {
                    const [a, b] = [...punteros.values()];
                    ultimaDist = Math.hypot(a.x - b.x, a.y - b.y);
                }
                refrescar(true);
            }, opt);

            canvas.addEventListener('pointermove', (e) => {
                const p = punteros.get(e.pointerId);
                if (!p) return;
                const k = escala();

                if (punteros.size === 1) {            // arrastrar
                    s.x += (e.clientX - p.x) * k;
                    s.y += (e.clientY - p.y) * k;
                    limitar(s);
                    punteros.set(e.pointerId, { x: e.clientX, y: e.clientY });
                } else if (punteros.size === 2) {     // pellizcar
                    punteros.set(e.pointerId, { x: e.clientX, y: e.clientY });
                    const [a, b] = [...punteros.values()];
                    const d = Math.hypot(a.x - b.x, a.y - b.y);
                    if (ultimaDist > 0) {
                        const r = canvas.getBoundingClientRect();
                        const cx = ((a.x + b.x) / 2 - r.left) * k;
                        const cy = ((a.y + b.y) / 2 - r.top) * k;
                        zoomEn(s, s.zoom * d / ultimaDist, cx, cy);
                    }
                    ultimaDist = d;
                }
                refrescar(true);
            }, opt);

            const soltar = (e) => { punteros.delete(e.pointerId); ultimaDist = 0; refrescar(false); };
            canvas.addEventListener('pointerup', soltar, opt);
            canvas.addEventListener('pointercancel', soltar, opt);

            canvas.addEventListener('wheel', (e) => {
                e.preventDefault();
                const r = canvas.getBoundingClientRect(), k = escala();
                zoomEn(s, s.zoom * (e.deltaY < 0 ? 1.08 : 1 / 1.08), (e.clientX - r.left) * k, (e.clientY - r.top) * k);
                refrescar(false);
            }, { signal: s.abort.signal, passive: false });

            if (slider) {
                slider.addEventListener('input', () => {
                    zoomEn(s, parseFloat(slider.value), W / 2, H / 2);
                    refrescar(false);
                }, opt);
            }
            refrescar(false);
        },

        reiniciar(id) {
            const s = estados[id];
            if (!s || !s.img) return;
            s.zoom = 1; centrar(s);
            if (s.refrescar) s.refrescar(false);
        },

        exportar(id) {
            const s = estados[id];
            if (!s || !s.img) return null;
            const c = document.createElement('canvas');
            c.width = 1200; c.height = 900;
            pintar(s, c, false);
            return c.toDataURL('image/jpeg', 0.88).split(',')[1];   // base64 sin prefijo
        },

        liberar(id) {
            const s = estados[id];
            if (s && s.abort) s.abort.abort();
            delete estados[id];
        }
    };
})();