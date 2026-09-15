namespace PWGAPG_Frontend.Services
{
    public class AuthService
    {
        public bool IsAuthenticated { get; private set; } = false;
        public string NombreEmpresa { get; set; } = "Mi Emprendimiento S.A.S.";
        public string DescripcionEmpresa { get; set; } = "Aquí puedes agregar una breve descripción de tu negocio, productos principales y horarios de atención.";
        public string UrlFotoPerfil { get; set; } = "images/default-avatar.png"; // O un placeholder SVG

        public event Action? OnChange;

        public void IniciarSesion()
        {
            IsAuthenticated = true;
            NotifyStateChanged();
        }

        public void CerrarSesion()
        {
            IsAuthenticated = false;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}