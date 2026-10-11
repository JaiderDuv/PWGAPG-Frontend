window.ayudaProveedor = {
    posicionar: function (anclaId, cajaId) {
        var ancla = document.getElementById(anclaId);
        var caja = document.getElementById(cajaId);
        if (!ancla || !caja) return null;
        var r = ancla.getBoundingClientRect();
        var antes = caja.style.display;
        caja.style.visibility = "hidden";
        caja.style.display = "block";
        var alto = caja.offsetHeight || 120;
        var ancho = caja.offsetWidth || 230;
        caja.style.display = antes;
        caja.style.visibility = "";
        var arriba = r.bottom + 6;
        if (arriba + alto > window.innerHeight - 8) {
            arriba = r.top - alto - 6;
        }
        if (arriba < 8) arriba = 8;
        var izquierda = r.left;
        if (izquierda + ancho > window.innerWidth - 8) {
            izquierda = window.innerWidth - ancho - 8;
        }
        if (izquierda < 8) izquierda = 8;
        return [arriba, izquierda];
    }
};
