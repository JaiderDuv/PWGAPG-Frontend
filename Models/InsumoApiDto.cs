namespace PWGAPG_Frontend.Models;

// Refleja el InsumoDto del BackEnd (GET /api/insumos).
// Cuadra con la tabla Insumo de la BD: sin categoría ni cantidad
// (esos campos no existen en la BD y no se agregaron).
public class InsumoApiDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal CostoUnitario { get; set; }
    public decimal PorcentajeMerma { get; set; }
    public string? Proveedor { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaActualizacion { get; set; }

    // > 0 significa que el DELETE será rechazado (409) por estar en uso.
    public int NumeroRecetas { get; set; }
}

public class ListaPaginadaDto<T>
{
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanoPagina { get; set; }
    public List<T> Datos { get; set; } = new();
}

// POST /api/insumos
public class CrearInsumoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public decimal CostoUnitario { get; set; }
    public decimal PorcentajeMerma { get; set; }
    public string? Proveedor { get; set; }
}

// PATCH /api/insumos/{id} — solo se envía lo que cambió (null = no tocar).
public class ActualizarInsumoRequest
{
    public string? Nombre { get; set; }
    public string? UnidadMedida { get; set; }
    public decimal? CostoUnitario { get; set; }
    public decimal? PorcentajeMerma { get; set; }
    public string? Proveedor { get; set; }
    public bool? Activo { get; set; }
}
