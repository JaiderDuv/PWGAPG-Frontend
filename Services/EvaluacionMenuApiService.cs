using System.Net.Http.Json;
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
        if (!res.IsSuccessStatusCode) return new(false, default, await ErrorRespuestaApi.LeerAsync(res));
        return new(true, await res.Content.ReadFromJsonAsync<T>(), null);
    }
}
