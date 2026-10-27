using System.Net.Http.Json;
using System.Text.Json;
using PWGAPG_Frontend.Models;

namespace PWGAPG_Frontend.Services;

public class SimuladorVentasApiService
{
    private readonly HttpClient _http;
    public SimuladorVentasApiService(HttpClient http) => _http = http;

    // Simulación mensual sin persistencia: el backend recalcula con los
    // precios y costos directos actuales del menú.
    public async Task<ApiResultado<SimulacionVentasDto>> CalcularAsync(
        IEnumerable<SimulacionVentaItemDto> productos)
    {
        var dto = new SimularVentasDto { Productos = productos.ToList() };
        return await Leer<SimulacionVentasDto>(
            await _http.PostAsJsonAsync("api/simulador-ventas/calcular", dto));
    }

    private static async Task<ApiResultado<T>> Leer<T>(HttpResponseMessage res)
    {
        if (!res.IsSuccessStatusCode) return new(false, default, await LeerError(res));
        return new(true, await res.Content.ReadFromJsonAsync<T>(), null);
    }

    private static async Task<string> LeerError(HttpResponseMessage res)
    {
        if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            return "Tu sesión expiró. Inicia sesión de nuevo.";
        try
        {
            using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());
            var root = doc.RootElement;
            if (root.TryGetProperty("mensaje", out var m)) return m.GetString() ?? "Error.";
            if (root.TryGetProperty("errors", out var errs))
                foreach (var campo in errs.EnumerateObject())
                    foreach (var msg in campo.Value.EnumerateArray())
                        return msg.GetString() ?? "Datos no válidos.";
        }
        catch { }
        return "Ocurrió un error inesperado.";
    }
}
