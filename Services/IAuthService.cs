using PWGAPG_Frontend.Models;

namespace PWGAPG_Frontend.Services
{
    public interface IAuthService
    {
        bool IsAuthenticated { get; }
        string NombreEmpresa { get; }
        string DescripcionEmpresa { get; }
        string UrlFotoPerfil { get; }

        event Action? OnChange;

        Task<LoginResponseDto?> Login(LoginDTO dto);
        Task<bool> RenovarSesionAsync();
        Task CerrarSesionAsync();

        Task<(bool exito, string mensaje)> ReenviarVerificacionAsync(string correo);
    }
}
