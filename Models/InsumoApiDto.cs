namespace PWGAPG_Frontend.Models;

// Refleja el InsumoDto del BackEnd (GET /api/insumos).
public class InsumoApiDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public string? Categoria { get; set; }
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
    public string? Categoria { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal PorcentajeMerma { get; set; }
    public string? Proveedor { get; set; }

    // Por defecto el formulario lo envía activado.
    public bool? Activo { get; set; } = true;
}

// PATCH /api/insumos/{id} — solo se envía lo que cambió (null = no tocar).
public class ActualizarInsumoRequest
{
    public string? Nombre { get; set; }
    public string? UnidadMedida { get; set; }
    public string? Categoria { get; set; }
    public decimal? CostoUnitario { get; set; }
    public decimal? PorcentajeMerma { get; set; }
    public string? Proveedor { get; set; }
    public bool? Activo { get; set; }
}

// GET /api/insumos/{id}/historial-precios — del más reciente al más viejo.
public class HistorialPrecioDto
{
    public DateTime Fecha { get; set; }
    public decimal Precio { get; set; }
    public string? Proveedor { get; set; }

    // % frente al precio anterior del mismo proveedor. Null si es el más antiguo.
    public decimal? VariacionPorcentual { get; set; }

    // Diferencia en dinero frente al precio anterior. Null si no hay previo.
    public decimal? Diferencia { get; set; }

    // true = subió (rojo), false = bajó (verde), null = sin variación o sin previo.
    public bool? EsSubida { get; set; }
}

public class HistorialPreciosDto
{
    public int InsumoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? ProveedorActual { get; set; }
    public decimal PrecioActual { get; set; }

    // false => solo se conoce el precio vigente (sin tabla de historial en BD).
    public bool HistorialCompletoDisponible { get; set; }
    public List<HistorialPrecioDto> Precios { get; set; } = new();
}

// GET /api/insumos/historial-precios — global del negocio.
public class HistorialPrecioGeneralDto
{
    public int InsumoId { get; set; }
    public string NombreInsumo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Precio { get; set; }
    public string? Proveedor { get; set; }
    public decimal? VariacionPorcentual { get; set; }
    public decimal? Diferencia { get; set; }
    public bool? EsSubida { get; set; }
}

public class HistorialGeneralDto
{
    public bool HistorialCompletoDisponible { get; set; }
    public List<HistorialPrecioGeneralDto> Precios { get; set; } = new();
}
