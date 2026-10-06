using PWGAPG_Frontend.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace PWGAPG_Frontend.Services;

public class RecetaApiService
{
    private readonly HttpClient _http;
    public RecetaApiService(HttpClient http) => _http = http;

    public async Task<ApiResultado<List<RecetaDto>>> ListarAsync(string? busqueda)
    {
        var url = string.IsNullOrWhiteSpace(busqueda)
            ? "api/recetas"
            : $"api/recetas?busqueda={Uri.EscapeDataString(busqueda)}";
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            return new(false, null, await LeerError(res));
        return new(true, await res.Content.ReadFromJsonAsync<List<RecetaDto>>(), null);
    }

    public async Task<List<InsumoOpcionDto>> ListarInsumosAsync()
    {
        // El backend limita tamanoPagina a 100.
        var r = await _http.GetFromJsonAsync<ListaInsumosDto>("api/insumos?tamanoPagina=100&ordenarPor=nombre");
        return r?.Datos ?? new();
    }

    public async Task<ApiResultado<RecetaDto>> CrearAsync(GuardarRecetaDto dto) =>
        await Enviar(() => _http.PostAsJsonAsync("api/recetas", dto));

    public async Task<ApiResultado<RecetaDto>> ActualizarAsync(int id, GuardarRecetaDto dto) =>
        await Enviar(() => _http.PutAsJsonAsync($"api/recetas/{id}", dto));

    public async Task<ApiResultado<bool>> EliminarAsync(int id)
    {
        var res = await _http.DeleteAsync($"api/recetas/{id}");
        return res.IsSuccessStatusCode
            ? new(true, true, null)
            : new(false, false, await LeerError(res));
    }

    private static async Task<ApiResultado<RecetaDto>> Enviar(Func<Task<HttpResponseMessage>> accion)
    {
        var res = await accion();
        if (!res.IsSuccessStatusCode)
            return new(false, null, await LeerError(res));
        return new(true, await res.Content.ReadFromJsonAsync<RecetaDto>(), null);
    }

    // Lee { mensaje } de tu backend o los errores de validación de ModelState.
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