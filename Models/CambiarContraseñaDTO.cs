using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    public class CambiarPasswordDTO
    {
        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
            ErrorMessage = "Debe contener al menos una mayúscula, una minúscula, un número y un carácter especial.")]
        public string NuevaPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debes confirmar la nueva contraseña.")]
        [Compare(nameof(NuevaPassword), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}