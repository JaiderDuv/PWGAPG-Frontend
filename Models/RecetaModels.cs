namespace PWGAPG_Frontend.Models;

public record ApiResultado<T>(bool Exito, T? Datos, string? Mensaje);

public static class Formato
{
    // Pesos colombianos sin depender de la cultura del navegador: $ 12.500
    public static string Moneda(decimal v) =>
        "$ " + decimal.Round(v, 0).ToString("#,##0", System.Globalization.CultureInfo.InvariantCulture).Replace(",", ".");
}
