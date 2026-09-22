namespace PWGAPG_Frontend.Models
{
    public class LoginResponseDto
    {
        public bool Exito { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int? IdUsuario { get; set; }
        public string? CorreoElectronico { get; set; }
        public string? NombreEmprendimiento { get; set; }
        public bool OnboardingCompletado { get; set; }
    }
}