using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(255, ErrorMessage = "El correo no puede superar 255 caracteres.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(128, ErrorMessage = "La contraseña no puede superar 128 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}