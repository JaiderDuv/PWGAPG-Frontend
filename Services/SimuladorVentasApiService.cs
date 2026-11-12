using System.Net.Http.Json;
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
        if (!res.IsSuccessStatusCode) return new(false, default, await ErrorRespuestaApi.LeerAsync(res));
        return new(true, await res.Content.ReadFromJsonAsync<T>(), null);
    }
}
