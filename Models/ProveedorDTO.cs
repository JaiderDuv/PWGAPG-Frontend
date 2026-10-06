using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    // Para crear un proveedor general del negocio (pantalla /proveedores)
    public class ProveedorFormDTO
    {
        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [RegularExpression(@"^$|^3\d{9}$", ErrorMessage = "El teléfono debe tener 10 dígitos y empezar por 3.")]
        public string? Telefono { get; set; }

        [RegularExpression(@"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string? Correo { get; set; }
    }

    // Para crear-y-asociar un proveedor nuevo directamente a un insumo (con costo)
    public class ProveedorNuevoDTO
    {
        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(150, ErrorMessage = "El nombre no puede superar los 150 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [RegularExpression(@"^$|^3\d{9}$", ErrorMessage = "El teléfono debe tener 10 dígitos y empezar por 3.")]
        public string? Telefono { get; set; }

        [RegularExpression(@"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string? Correo { get; set; }

        [Required(ErrorMessage = "El costo es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0.")]
        public decimal Costo { get; set; }
    }

    public class ProveedorDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
    }

    public class InsumoProveedorDTO
    {
        public int ProveedorId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Correo { get; set; }
        public decimal Costo { get; set; }
        public bool EsPrincipal { get; set; }
        public bool EsMasBarato { get; set; }
    }
}