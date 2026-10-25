using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models
{
    public sealed class CantidadSegunUnidadAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> UnidadesPequenas =
            new(StringComparer.OrdinalIgnoreCase) { "unidad", "taza" };

        private readonly string _propiedadUnidad;

        public CantidadSegunUnidadAttribute(string propiedadUnidad)
        {
            _propiedadUnidad = propiedadUnidad;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not decimal cantidad)
                return ValidationResult.Success;

            var unidadProp = validationContext.ObjectType.GetProperty(_propiedadUnidad);
            var unidad = unidadProp?.GetValue(validationContext.ObjectInstance) as string;

            var maximo = !string.IsNullOrWhiteSpace(unidad) && UnidadesPequenas.Contains(unidad.Trim())
                ? 999.99m
                : 999999.99m;

            if (cantidad <= 0)
                return new ValidationResult("La cantidad debe ser mayor a 0.", new[] { validationContext.MemberName! });

            if (cantidad > maximo)
                return new ValidationResult($"La cantidad no puede superar {maximo:N2}.", new[] { validationContext.MemberName! });

            if (Math.Round(cantidad * 100m) / 100m != cantidad)
                return new ValidationResult("La cantidad admite máximo 2 decimales.", new[] { validationContext.MemberName! });

            return ValidationResult.Success;
        }
    }

    public class ProveedorFormDTO
    {
        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(75, ErrorMessage = "El nombre no puede superar los 75 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [RegularExpression(@"^$|^3\d{9}$", ErrorMessage = "El teléfono debe tener 10 dígitos y empezar por 3.")]
        public string? Telefono { get; set; }

        [StringLength(255, ErrorMessage = "El correo no puede superar los 255 caracteres.")]
        [RegularExpression(@"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string? Correo { get; set; }
    }

    public class ProveedorNuevoDTO
    {
        [Required(ErrorMessage = "El nombre del proveedor es obligatorio.")]
        [StringLength(75, ErrorMessage = "El nombre no puede superar los 75 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [RegularExpression(@"^$|^3\d{9}$", ErrorMessage = "El teléfono debe tener 10 dígitos y empezar por 3.")]
        public string? Telefono { get; set; }

        [StringLength(255, ErrorMessage = "El correo no puede superar los 255 caracteres.")]
        [RegularExpression(@"^$|^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Ingrese un correo electrónico válido.")]
        public string? Correo { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [CantidadSegunUnidad(nameof(UnidadMedida))]
        public decimal Cantidad { get; set; } = 1;
        public string CantidadTexto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La unidad es obligatoria.")]
        public string UnidadMedida { get; set; } = "g";

        [Required(ErrorMessage = "El costo es obligatorio.")]
        [Range(0.01, 999999.99, ErrorMessage = "El costo debe estar entre 0,01 y 999.999,99.")]
        [MaximoDecimales(2)]
        public decimal Costo { get; set; }
        public string CostoTexto { get; set; } = string.Empty;
    }
}
