using System.ComponentModel.DataAnnotations;

namespace PWGAPG_Frontend.Models;

// ---------- Menú / Producto ----------
public class RecetaEnProductoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona una receta.")]
    public int RecetaId { get; set; }

    // Porciones de esta receta que entran en el producto:
    // costo receta = (costo de insumos ÷ porciones base) × porciones.
    [Range(0.001, 10000, ErrorMessage = "La cantidad debe estar entre 0.001 y 10000.")]
    public decimal Cantidad { get; set; } = 1;
}

public class InsumoDirectoDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Selecciona un insumo.")]
    public int InsumoId { get; set; }

    [Range(0.001, 100000, ErrorMessage = "La cantidad debe estar entre 0.001 y 100000.")]
    public decimal Cantidad { get; set; } = 1;

    [Required(ErrorMessage = "La unidad es obligatoria.")]
    [RegularExpression(@"(?i)^(g|kg|ml|l|oz|lb|unidad|taza)$", ErrorMessage = "Unidad no válida.")]
    public string UnidadMedida { get; set; } = "unidad";
}

public class RecetaDetalleProductoDto
{
    public int RecetaId { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Cantidad { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal CostoAportado { get; set; }
}

public class InsumoDirectoDetalleDto
{
    public int InsumoId { get; set; }
    public string Nombre { get; set; } = "";
    public decimal Cantidad { get; set; }
    public string UnidadReceta { get; set; } = "";
    public string UnidadInsumo { get; set; } = "";
    public decimal CostoAportado { get; set; }
    public string FuenteCosto { get; set; } = "";
}

public class ProductoMenuDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public decimal? PrecioVenta { get; set; }
    public decimal CostoTotal { get; set; }

    // Costo directo = recetas + empaques, sin costos fijos.
    public decimal CostoDirecto { get; set; }

    // Cuota de costos fijos prorrateada incluida en CostoTotal.
    public decimal CostoFijoProrrateado { get; set; }

    public decimal? MargenReal { get; set; }

    // Margen calculado solo con el costo directo (recetas + empaques).
    // Es la base del menú y de la sugerencia de precio por defecto.
    public decimal? MargenDirecto { get; set; }

    public int NumRecetas { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class ProductoMenuDetalleDto : ProductoMenuDto
{
    public List<RecetaDetalleProductoDto> Recetas { get; set; } = new();
    public List<InsumoDirectoDetalleDto> InsumosDirectos { get; set; } = new();
    public decimal CostoRecetas { get; set; }
    public decimal CostoInsumosDirectos { get; set; }
    public List<string> Advertencias { get; set; } = new();
}

public class CrearProductoMenuDto
{
    public string Nombre { get; set; } = "";
    public decimal? PrecioVenta { get; set; }
    public bool Activo { get; set; } = true;
    public List<RecetaEnProductoDto> Recetas { get; set; } = new();

    // Empaques (insumos directos) que se guardan con el producto y entran
    // en su costo real. Lista vacía si no hay.
    public List<InsumoDirectoDto> InsumosDirectos { get; set; } = new();
}

public class ActualizarProductoMenuDto
{
    public string Nombre { get; set; } = "";
    public decimal? PrecioVenta { get; set; }
    public bool Activo { get; set; } = true;
    public List<RecetaEnProductoDto> Recetas { get; set; } = new();

    public List<InsumoDirectoDto> InsumosDirectos { get; set; } = new();
}

public class PatchProductoMenuDto
{
    public string? Nombre { get; set; }
    public decimal? PrecioVenta { get; set; }
    public bool? QuitarPrecioVenta { get; set; }
    public bool? Activo { get; set; }
    public List<RecetaEnProductoDto>? Recetas { get; set; }

    // null = no cambia; lista (aunque vacía) = reemplaza los empaques.
    public List<InsumoDirectoDto>? InsumosDirectos { get; set; }
}

public class CostoProductoDto
{
    public int ProductoId { get; set; }
    public string Producto { get; set; } = "";
    public decimal CostoRecetas { get; set; }
    public decimal CostoInsumosDirectos { get; set; }

    // Costo directo = recetas + empaques (sin fijos).
    public decimal CostoDirecto { get; set; }

    // Cuota mensual de costos fijos prorrateada.
    public decimal CostoFijoProrrateado { get; set; }

    // Preparaciones base que consume el producto:
    // Σ (porciones usadas ÷ porciones base de cada receta).
    public decimal PreparacionesEquivalentes { get; set; }

    public decimal CostoTotal { get; set; }
    public decimal? PrecioVenta { get; set; }
    public decimal? MargenReal { get; set; }

    // Margen sobre el precio de venta usando solo el costo directo (sin fijos).
    public decimal? MargenDirecto { get; set; }

    public bool SinCostosFijos { get; set; }
    public List<RecetaDetalleProductoDto> Recetas { get; set; } = new();
    public List<InsumoDirectoDetalleDto> InsumosDirectos { get; set; } = new();
    public List<string> Advertencias { get; set; } = new();
}

public class PrecioSugeridoProductoDto
{
    public int? ProductoId { get; set; }
    public string Producto { get; set; } = "";

    // Costo base usado para el precio sugerido: directo si no se incluyeron
    // fijos, o completo si se incluyeron.
    public decimal CostoTotal { get; set; }

    public decimal CostoDirecto { get; set; }
    public decimal CostoFijoProrrateado { get; set; }
    public bool IncluyeCostosFijos { get; set; }

    public decimal MargenDeseado { get; set; }
    public decimal? PrecioSugerido { get; set; }
    public bool MargenValido { get; set; }
    public List<string> Advertencias { get; set; } = new();
}

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
        [RegularExpression(@"(?i)^(g|kg|ml|l|oz|lb|unidad|taza)$", ErrorMessage = "Unidad no válida.")]
        public string UnidadMedida { get; set; } = "unidad";
    }
}
