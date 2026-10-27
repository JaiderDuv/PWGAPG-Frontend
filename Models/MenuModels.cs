using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models;

// ---------- Formulario con validaciones estilo Recetas/CostosFijos ----------
public class ProductoMenuFormModelo
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(75, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 75 caracteres.")]
    [RegularExpression(@"^(?=.*\p{L})[\p{L}\p{N} .,'\-()&]+$",
        ErrorMessage = "Solo letras, números, espacios y . , ' - ( ) & (debe incluir letras).")]
    public string Nombre { get; set; } = "";

    // Editable en el formulario: con validación de 6 dígitos y aviso en español.
    [Range(0.01, 999999.99, ErrorMessage = "El precio de venta no puede exceder los 6 dígitos (máximo 999.999,99).")]
    public decimal? PrecioVenta { get; set; }

    public bool Activo { get; set; } = true;

    public List<FilaReceta> FilasReceta { get; set; } = new();

    public List<FilaEmpaque> FilasEmpaque { get; set; } = new();

    public class FilaReceta
    {
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona una receta.")]
        public int RecetaId { get; set; }

        [Range(0.001, 10000, ErrorMessage = "La cantidad debe estar entre 0.001 y 10000.")]
        public decimal Cantidad { get; set; } = 1;
    }

    public class FilaEmpaque
    {
        [Range(1, int.MaxValue, ErrorMessage = "Selecciona un insumo.")]
        public int InsumoId { get; set; }

        [Range(0.001, 100000, ErrorMessage = "La cantidad debe estar entre 0.001 y 100000.")]
        public decimal Cantidad { get; set; } = 1;

        [Required(ErrorMessage = "La unidad es obligatoria.")]
        [RegularExpression(ConversorUnidades.PatronUnidades, ErrorMessage = "Unidad no válida.")]
        public string UnidadMedida { get; set; } = "unidad";
    }
}
