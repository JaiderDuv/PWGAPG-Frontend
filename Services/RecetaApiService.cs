using PWGAPG_Frontend.Models;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace PWGAPG_Frontend.Services;

public class RecetaApiService
{
    // Convierte "/uploads/recetas/x.jpg" en la URL absoluta del backend.
    public string? UrlImagen(string? ruta) =>
        string.IsNullOrWhiteSpace(ruta) || _http.BaseAddress is null
            ? null
            : new Uri(_http.BaseAddress, ruta).ToString();

    public async Task<ApiResultado<RecetaDto>> SubirImagenAsync(int id, byte[] bytes, string nombre, string contentType)
    {
        using var contenido = new MultipartFormDataContent();
        var archivo = new ByteArrayContent(bytes);
        archivo.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        contenido.Add(archivo, "archivo", nombre);   // "archivo" = nombre del parámetro del controller
        return await Enviar(() => _http.PostAsync($"api/recetas/{id}/imagen", contenido));
    }

    public async Task<ApiResultado<RecetaDto>> EliminarImagenAsync(int id) =>
        await Enviar(() => _http.DeleteAsync($"api/recetas/{id}/imagen"));
    private readonly HttpClient _http;
    public RecetaApiService(HttpClient http) => _http = http;

    public async Task<ApiResultado<List<RecetaDto>>> ListarAsync(string? busqueda)
    {
        var url = string.IsNullOrWhiteSpace(busqueda)
            ? "api/recetas"
            : $"api/recetas?busqueda={Uri.EscapeDataString(busqueda)}";
        var res = await _http.GetAsync(url);
        if (!res.IsSuccessStatusCode)
            return new(false, null, await ErrorRespuestaApi.LeerAsync(res));
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
            : new(false, false, await ErrorRespuestaApi.LeerAsync(res));
    }

    private static async Task<ApiResultado<RecetaDto>> Enviar(Func<Task<HttpResponseMessage>> accion)
    {
        var res = await accion();
        if (!res.IsSuccessStatusCode)
            return new(false, null, await ErrorRespuestaApi.LeerAsync(res));
        return new(true, await res.Content.ReadFromJsonAsync<RecetaDto>(), null);
    }
}
