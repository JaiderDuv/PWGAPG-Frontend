using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    public class RegistroDTO
    {
        [Required(ErrorMessage = "Debe indicar si el negocio está formalizado.")]
        public string Formalizado { get; set; } = "no"; // "si" o "no"

        // No llevan [Required] aquí; la lógica de si es obligatorio o no para NIT y Razón Social 
        // se evalúa en el botón de envío según el valor de 'Formalizado'
        public string? Nit { get; set; }
        public string? RazonSocial { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un tipo de documento.")]
        public string TipoDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El documento de identidad es obligatorio.")]
        public string DocumentoIdentidad { get; set; } = string.Empty;

        // Este campo es opcional (se puede completar después)
        public string? NombreEmprendimiento { get; set; }

        [Required(ErrorMessage = "Seleccione una categoría de negocio.")]
        public string CategoriaNegocio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono de contacto es obligatorio.")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido.")]
        public string TelefonoContacto { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo de contacto es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo de contacto válido.")]
        public string CorreoContacto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Password { get; set; } = string.Empty;
    }
}