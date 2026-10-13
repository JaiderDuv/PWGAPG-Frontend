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
    public decimal MontoMensual { get; set; }
    public string UnidadProrrateo { get; set; } = "";
    public decimal ValorDivisor { get; set; }
    public bool Activo { get; set; }
}

public class CrearCostoFijoDto
{
    public string Nombre { get; set; } = "";
    public decimal MontoMensual { get; set; }
    public string UnidadProrrateo { get; set; } = "unidades_producidas";
    public decimal? ValorDivisor { get; set; }   // obligatorio: ya no tiene default 1
    public bool Activo { get; set; } = true;
}

public class ActualizarCostoFijoDto
{
    public string? Nombre { get; set; }
    public decimal? MontoMensual { get; set; }
    public string? UnidadProrrateo { get; set; }
    public decimal? ValorDivisor { get; set; }
    public bool? Activo { get; set; }
}