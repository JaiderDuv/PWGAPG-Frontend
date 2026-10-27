namespace PWGAPG_Frontend.Models;

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
