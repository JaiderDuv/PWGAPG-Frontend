namespace PWGAPG_Frontend.Models;

public class RecetaInsumoDto
{
    public int InsumoId { get; set; }
    public string? InsumoNombre { get; set; }
    public decimal Cantidad { get; set; }
    public string UnidadMedida { get; set; } = "g";
    public decimal CostoLinea { get; set; }
}

public class GuardarRecetaDto
{
    public string Nombre { get; set; } = "";
    public int PorcionesBase { get; set; } = 1;
    public string? Instrucciones { get; set; }
    public string? Categoria { get; set; }
    public List<RecetaInsumoDto> Insumos { get; set; } = new();
}

public class RecetaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public int PorcionesBase { get; set; }
    public string? Instrucciones { get; set; }
    public string? Categoria { get; set; }
    public string? ImagenUrl { get; set; }
    public DateTime FechaCreacion { get; set; }
    public decimal CostoTotal { get; set; }
    public decimal CostoPorPorcion { get; set; }
    public List<RecetaInsumoDto> Insumos { get; set; } = new();
}

public class InsumoOpcionDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = "";
    public string UnidadMedida { get; set; } = "";
    public decimal? Cantidad { get; set; }          // tamaño del paquete; CostoUnitario es el precio del paquete
    public decimal CostoUnitario { get; set; }
    public decimal PorcentajeMerma { get; set; }
}

public class ListaInsumosDto
{
    public List<InsumoOpcionDto> Datos { get; set; } = new();
}

public record ApiResultado<T>(bool Exito, T? Datos, string? Mensaje);

public static class Formato
{
    // Pesos colombianos sin depender de la cultura del navegador: $ 12.500
    public static string Moneda(decimal v) =>
        "$ " + decimal.Round(v, 0).ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture).Replace(",", ".");
}

// Los valores se guardan en la BD; las etiquetas son solo para mostrar.
public static class CategoriasReceta
{
    public static readonly IReadOnlyList<(string Valor, string Etiqueta)> Lista = new List<(string, string)>
    {
        ("entradas", "Entradas"),
        ("platos_fuertes", "Platos fuertes"),
        ("postres", "Postres"),
        ("bebidas", "Bebidas"),
        ("salsas_bases", "Salsas y bases"),
        ("panaderia", "Panadería y repostería"),
        ("otros", "Otros")
    };

    public static string Etiqueta(string? valor) =>
        Lista.FirstOrDefault(c => c.Valor == valor).Etiqueta ?? "Sin categoría";

    public static int Orden(string? valor)
    {
        for (var i = 0; i < Lista.Count; i++)
            if (Lista[i].Valor == valor) return i;
        return int.MaxValue;
    }
}