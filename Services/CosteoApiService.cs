using System.Net.Http.Json;
using System.Text.Json;
using PWGAPG_Frontend.Models;

namespace PWGAPG_Frontend.Services;

public class CosteoApiService
{
    private readonly HttpClient _http;
    public CosteoApiService(HttpClient http) => _http = http;

    public async Task<ApiResultado<CostoRecetaDto>> ObtenerCostoAsync(int recetaId, IEnumerable<int>? costosFijosIds = null)
    {
        // Sin selección (null) = todos. Selección vacía se envía como -1 (id imposible)
        // porque una query sin valores no permite distinguir "ninguno" de "todos".
        var ids = costosFijosIds is null ? null
            : (costosFijosIds.Any() ? costosFijosIds : new[] { -1 });
        var url = $"api/recetas/{recetaId}/costo";
        if (ids is not null)
            url += "?" + string.Join("&", ids.Select(id => $"costosFijosIds={id}"));
        return await Leer<CostoRecetaDto>(await _http.GetAsync(url));
    }

    public async Task<ApiResultado<EscaladoRecetaDto>> ObtenerEscaladoAsync(int recetaId, int porciones) =>
        await Leer<EscaladoRecetaDto>(await _http.GetAsync($"api/recetas/{recetaId}/escalado?porciones={porciones}"));

    public async Task<ApiResultado<List<CostoFijoDto>>> ListarCostosFijosAsync() =>
        await Leer<List<CostoFijoDto>>(await _http.GetAsync("api/costos-fijos?incluirInactivos=true"));

    public async Task<ApiResultado<CostoFijoDto>> CrearCostoFijoAsync(CrearCostoFijoDto dto) =>
        await Leer<CostoFijoDto>(await _http.PostAsJsonAsync("api/costos-fijos", dto));

    public async Task<ApiResultado<CostoFijoDto>> ActualizarCostoFijoAsync(int id, ActualizarCostoFijoDto dto) =>
        await Leer<CostoFijoDto>(await _http.PatchAsJsonAsync($"api/costos-fijos/{id}", dto));

    public async Task<ApiResultado<bool>> EliminarCostoFijoAsync(int id)
    {
        var res = await _http.DeleteAsync($"api/costos-fijos/{id}");
        return res.IsSuccessStatusCode
            ? new(true, true, null)
            : new(false, false, await LeerError(res));
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