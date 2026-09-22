using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    public class SocioDTO
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? FotoPerfilBase64 { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombres { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellidos { get; set; } = string.Empty;

        [Required(ErrorMessage = "Seleccione un tipo de documento.")]
        public string TipoDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        [Phone(ErrorMessage = "Ingrese un número válido.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        public string CorreoElectronico { get; set; } = string.Empty;
    }
}