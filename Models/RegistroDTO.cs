using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    public class RegistroDTO
    {
        [Required(ErrorMessage = "Debe indicar si el negocio está formalizado.")]
        public bool Formalizado { get; set; }

        // NIT: obligatorio solo si Formalizado = true (se valida manualmente en Registro.razor)
        [StringLength(20)]
        public string? Nit { get; set; }

        [StringLength(150, ErrorMessage = "La Razón Social no puede superar 150 caracteres.")]
        public string? RazonSocial { get; set; }

        // Correo Principal
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "El formato del correo electrónico no es válido.")]
        public string CorreoElectronico { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un tipo de documento.")]
        [StringLength(5)]
        public string TipoDocumento { get; set; } = string.Empty;

        // Documento de Identidad: Solo números, entre 5 y 12 dígitos
        [Required(ErrorMessage = "El documento de identidad es obligatorio.")]
        [StringLength(20)]
        [RegularExpression(@"^\d{5,12}$", ErrorMessage = "El documento debe contener únicamente números y debe tener una longitud de entre 5 y 12 dígitos.")]
        public string DocumentoIdentidadUe { get; set; } = string.Empty;

        
        public string? NombreEmprendimiento { get; set; }

        [Required(ErrorMessage = "Seleccione una categoría de negocio.")]
        [StringLength(100)]
        public string CategoriaNegocio { get; set; } = string.Empty;

        // Teléfono: Solo números, exactamente 10 dígitos
        [Required(ErrorMessage = "El teléfono de contacto es obligatorio.")]
        [StringLength(20)]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "El teléfono debe ser numérico y contener exactamente 10 dígitos.")]
        public string TelefonoContacto { get; set; } = string.Empty;

        // Correo Contacto
        [Required(ErrorMessage = "El correo de contacto es obligatorio.")]
        [StringLength(255)]
        [EmailAddress(ErrorMessage = "Ingrese un correo de contacto válido.")]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "El formato del correo de contacto no es válido.")]
        public string CorreoElectronicoContacto { get; set; } = string.Empty;

        // Contraseña
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "La contraseña debe tener entre 8 y 128 caracteres.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, una letra minúscula, un número y un carácter especial.")]
        public string HashContrasena { get; set; } = string.Empty;
    }
}