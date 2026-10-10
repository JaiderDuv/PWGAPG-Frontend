namespace PWGAPG_Frontend.Models;

public static class ConversorUnidades
{
    private static readonly Dictionary<string, (string Familia, decimal Factor)> Tabla =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["g"] = ("masa", 1m),
            ["kg"] = ("masa", 1000m),
            ["oz"] = ("masa", 28.3495m),
            ["lb"] = ("masa", 453.592m),
            ["ml"] = ("volumen", 1m),
            ["l"] = ("volumen", 1000m),
            ["taza"] = ("volumen", 240m),
            ["unidad"] = ("conteo", 1m)
        };

    public static IEnumerable<string> UnidadesDeLaMismaFamilia(string unidad) =>
        Tabla.TryGetValue(unidad, out var x)
            ? Tabla.Where(t => t.Value.Familia == x.Familia).Select(t => t.Key)
            : Tabla.Keys;

    public static decimal CostoLinea(decimal cantidad, string unidadReceta, string unidadInsumo,
                                     decimal costoUnitario, decimal mermaPorcentaje)
    {
        if (!Tabla.TryGetValue(unidadReceta, out var a) || !Tabla.TryGetValue(unidadInsumo, out var b)
            || a.Familia != b.Familia) return 0m;
        var enUnidadInsumo = cantidad * a.Factor / b.Factor;
        var rendimiento = Math.Max(1m - mermaPorcentaje / 100m, 0.01m);
        return decimal.Round(enUnidadInsumo * costoUnitario / rendimiento, 2);
    }
}