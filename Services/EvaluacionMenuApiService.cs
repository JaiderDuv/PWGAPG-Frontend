using System.Net.Http.Json;
using System.Text.Json;
using PWGAPG_Frontend.Models;

namespace PWGAPG_Frontend.Services;

public class EvaluacionMenuApiService
{
    private readonly HttpClient _http;
    public EvaluacionMenuApiService(HttpClient http) => _http = http;

    public async Task<ApiResultado<UmbralMargenDto>> ObtenerUmbralAsync() =>
        await Leer<UmbralMargenDto>(await _http.GetAsync("api/menu/umbral"));

    public async Task<ApiResultado<UmbralMargenDto>> ActualizarUmbralAsync(decimal umbral) =>
        await Leer<UmbralMargenDto>(await _http.PutAsJsonAsync("api/menu/umbral", new { umbralMargenMinimo = umbral }));

    public async Task<ApiResultado<EvaluacionMenuDto>> ObtenerEvaluacionAsync() =>
        await Leer<EvaluacionMenuDto>(await _http.GetAsync("api/menu/evaluacion"));

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