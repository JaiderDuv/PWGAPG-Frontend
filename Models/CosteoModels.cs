namespace PWGAPG_Frontend.Models;

public class LineaCostoDto
{
    public int InsumoId { get; set; }
    public string Insumo { get; set; } = "";
    public decimal Cantidad { get; set; }
    public string UnidadReceta { get; set; } = "";
    public string UnidadInsumo { get; set; } = "";
    public decimal CantidadConvertida { get; set; }
    public decimal CantidadParaComprar { get; set; }
    public decimal CostoProveedor { get; set; }
    public string FuenteCosto { get; set; } = "proveedor_principal";
    public decimal CostoInsumo { get; set; }
    public decimal MermaExtra { get; set; }
}

public class CostoRecetaDto
{
    public int RecetaId { get; set; }
    public string Receta { get; set; } = "";
    public int PorcionesBase { get; set; }
    public decimal CostoInsumos { get; set; }
    public decimal CostoMermaAplicada { get; set; }
    public decimal CostoFijoProrrateado { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal CostoPorPorcion { get; set; }
    public bool SinCostosFijos { get; set; }
    public List<string> Advertencias { get; set; } = new();
    public List<LineaCostoDto> Lineas { get; set; } = new();
}

public class EscaladoRecetaDto
{
    public int RecetaId { get; set; }
    public string Receta { get; set; } = "";
    public int PorcionesBase { get; set; }
    public int PorcionesDeseadas { get; set; }
    public decimal FactorEscala { get; set; }
    public decimal CostoInsumos { get; set; }
    public decimal CostoMermaAplicada { get; set; }
    public decimal CostoFijoProrrateado { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal CostoPorPorcion { get; set; }
    public bool SinCostosFijos { get; set; }
    public List<string> Advertencias { get; set; } = new();
    public List<LineaCostoDto> Lineas { get; set; } = new();
}

public class CostoFijoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string Categoria { get; set; } = "Otros";
    public decimal MontoMensual { get; set; }
    public string UnidadProrrateo { get; set; } = "";
    public decimal ValorDivisor { get; set; }
    public bool Activo { get; set; }
}

// Punto de equilibrio mensual: ventas mínimas para cubrir los costos fijos.
// El costo directo no incluye fijos; el margen de contribución = precio − directo.
public class PuntoEquilibrioDto
{
    public decimal CostosFijosMensuales { get; set; }
    public int ProductosConsiderados { get; set; }
    public decimal? MargenContribucionPromedioPorcentaje { get; set; }
    public decimal? VentasEquilibrioMensuales { get; set; }
    public List<PuntoEquilibrioProductoDto> Productos { get; set; } = new();
    public List<string> Advertencias { get; set; } = new();
}

public class PuntoEquilibrioProductoDto
{
    public int ProductoId { get; set; }
    public string Producto { get; set; } = "";
    public decimal PrecioVenta { get; set; }
    public decimal CostoDirecto { get; set; }
    public decimal MargenContribucion { get; set; }
    public decimal MargenContribucionPorcentaje { get; set; }
    public decimal? UnidadesEquilibrio { get; set; }
    public decimal? VentasEquilibrio { get; set; }
}

public class CrearCostoFijoDto
{
    public string Nombre { get; set; } = "";
    public string Categoria { get; set; } = "Otros";
    public decimal MontoMensual { get; set; }
    public string UnidadProrrateo { get; set; } = "unidades_producidas";
    public decimal? ValorDivisor { get; set; }   // obligatorio: ya no tiene default 1
    public bool Activo { get; set; } = true;
}

public class ActualizarCostoFijoDto
{
    public string? Nombre { get; set; }
    public string? Categoria { get; set; }
    public decimal? MontoMensual { get; set; }
    public string? UnidadProrrateo { get; set; }
    public decimal? ValorDivisor { get; set; }
    public bool? Activo { get; set; }
}