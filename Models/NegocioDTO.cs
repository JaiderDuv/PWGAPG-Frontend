using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    public class NegocioDTO
    {
        public string? LogoBase64 { get; set; }

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        public string NombreEmpresa { get; set; } = string.Empty;

        public string? Nit { get; set; }
        public string? RazonSocial { get; set; }

        [Required(ErrorMessage = "El campo RUES es obligatorio.")]
        public string Rues { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo de la empresa es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo válido.")]
        public string CorreoEmpresa { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono de la empresa es obligatorio.")]
        [Phone(ErrorMessage = "Ingrese un número válido.")]
        public string TelefonoEmpresa { get; set; } = string.Empty;

        public bool TieneSitioFisico { get; set; } = false;

        public string? Direccion { get; set; }
    }
}