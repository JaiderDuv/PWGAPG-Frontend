using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models;

public class PerfilEmprendedorDto
{
    [Required(ErrorMessage = "El correo de la cuenta es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo de cuenta válido.")]
    [StringLength(255)]
    public string CorreoCuenta { get; set; } = string.Empty;

    [StringLength(150)]
    public string NombreEmprendimiento { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona una categoría de negocio.")]
    [StringLength(100)]
    public string CategoriaNegocio { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo de contacto es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo de contacto válido.")]
    [StringLength(255)]
    public string CorreoContacto { get; set; } = string.Empty;

    [StringLength(20)]
    public string Nit { get; set; } = string.Empty;

    [StringLength(150)]
    public string RazonSocial { get; set; } = string.Empty;

    [StringLength(50)]
    public string Rues { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo de la empresa es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo de empresa válido.")]
    [StringLength(255)]
    public string CorreoEmpresa { get; set; } = string.Empty;

    public bool Formalizado { get; set; }

    public bool TieneSitioFisico { get; set; }

    [StringLength(255)]
    public string Direccion { get; set; } = string.Empty;

    public List<SocioNegocioDto> Socios { get; set; } = new();
}

public class SocioNegocioDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Los nombres del socio son obligatorios.")]
    [StringLength(100)]
    public string Nombres { get; set; } = string.Empty;

    [Required(ErrorMessage = "Los apellidos del socio son obligatorios.")]
    [StringLength(100)]
    public string Apellidos { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona el tipo de documento del socio.")]
    [StringLength(5)]
    public string TipoDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El documento del socio es obligatorio.")]
    [StringLength(20)]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono del socio es obligatorio.")]
    [Phone(ErrorMessage = "Ingresa un teléfono válido.")]
    [StringLength(20)]
    public string Telefono { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo del socio es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
    [StringLength(255)]
    public string CorreoElectronico { get; set; } = string.Empty;
}

// Perfil de usuario: edición libre, sin validaciones. Se autocompleta al abrir.
public class PerfilUsuarioDto
{
    public string? FotoPerfilUrl { get; set; }
    public string TipoDocumento { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string TelefonoContacto { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
}

public class ConfirmarEdicionPerfilDto
{
    [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}

public class PermisoEdicionPerfilDto
{
    public string EditToken { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}

public class ActualizarPerfilResultadoDto
{
    public bool Exito { get; set; }
    public bool RequiereReautenticacion { get; set; }
    public bool RequiereConfirmacionCorreo { get; set; }
    public string Mensaje { get; set; } = string.Empty;
}
