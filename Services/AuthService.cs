namespace PWGAPG_Frontend.Services
{
    public class AuthService
    {
        public bool IsAuthenticated { get; private set; } = false;

        public string NombreEmpresa { get; set; } = "Mi Emprendimiento S.A.S.";

        public string DescripcionEmpresa { get; set; } = "Aquí puedes agregar una breve descripción de tu negocio, productos principales y horarios de atención.";

        public string UrlFotoPerfil { get; set; } = "images/default-avatar.png"; 

        public event Action? OnChange;

        public void IniciarSesion(string nombreEmpresa, string descripcionEmpresa = "", string urlFoto = "images/default-avatar.png")
        {
            IsAuthenticated = true;
            NombreEmpresa = nombreEmpresa;
            DescripcionEmpresa = string.IsNullOrWhiteSpace(descripcionEmpresa)
                ? "Sin descripción registrada."
                : descripcionEmpresa;
            UrlFotoPerfil = string.IsNullOrWhiteSpace(urlFoto)
                ? "images/default-avatar.png"
                : urlFoto;

            NotifyStateChanged();
        }

        public void CerrarSesion()
        {
            IsAuthenticated = false;
            NombreEmpresa = string.Empty;
            DescripcionEmpresa = string.Empty;
            UrlFotoPerfil = "images/default-avatar.png";

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}