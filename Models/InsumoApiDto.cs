namespace PWGAPG_Frontend.Models;

public class InsumoApiDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal PorcentajeMerma { get; set; }
    public decimal? Cantidad { get; set; }
    public string? Proveedor { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaActualizacion { get; set; }

    public int NumeroRecetas { get; set; }
}

public class ListaPaginadaDto<T>
{
    public int Total { get; set; }
    public int Pagina { get; set; }
    public int TamanoPagina { get; set; }
    public List<T> Datos { get; set; } = new();
}

public class CrearInsumoRequest
{
    public string Nombre { get; set; } = string.Empty;
    public string UnidadMedida { get; set; } = string.Empty;
    public string? Categoria { get; set; }
    public decimal CostoUnitario { get; set; }
    public decimal PorcentajeMerma { get; set; }
    public decimal? Cantidad { get; set; }
    public string? Proveedor { get; set; }

    public bool? Activo { get; set; } = true;
}

public class ActualizarInsumoRequest
{
    public string? Nombre { get; set; }
    public string? UnidadMedida { get; set; }
    public string? Categoria { get; set; }
    public decimal? CostoUnitario { get; set; }
    public decimal? PorcentajeMerma { get; set; }
    public decimal? Cantidad { get; set; }
    public string? Proveedor { get; set; }
    public bool? Activo { get; set; }
}

public class HistorialPrecioDto
{
    public DateTime Fecha { get; set; }
    public decimal Precio { get; set; }
    public string? Proveedor { get; set; }
    public decimal? Cantidad { get; set; }
    public decimal? PrecioUnitario { get; set; }

    public decimal? VariacionPorcentual { get; set; }

    public decimal? Diferencia { get; set; }

    public bool? EsSubida { get; set; }
}

public class HistorialPreciosDto
{
    public int InsumoId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? ProveedorActual { get; set; }
    public decimal PrecioActual { get; set; }

    public bool HistorialCompletoDisponible { get; set; }
    public bool InsumoEliminado { get; set; }
    public List<HistorialPrecioDto> Precios { get; set; } = new();
}

public class HistorialPrecioGeneralDto
{
    public int InsumoId { get; set; }
    public string NombreInsumo { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
    public decimal Precio { get; set; }
    public string? Proveedor { get; set; }
    public decimal? Cantidad { get; set; }
    public decimal? PrecioUnitario { get; set; }
    public decimal? VariacionPorcentual { get; set; }
    public decimal? Diferencia { get; set; }
    public bool? EsSubida { get; set; }
    public bool InsumoEliminado { get; set; }
}

public class HistorialGeneralDto
{
    public bool HistorialCompletoDisponible { get; set; }
    public List<HistorialPrecioGeneralDto> Precios { get; set; } = new();
}
