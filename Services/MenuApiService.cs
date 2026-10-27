using System.Net.Http.Json;
using System.Text.Json;
using PWGAPG_Frontend.Models;

namespace PWGAPG_Frontend.Services;

public class MenuApiService
{
    private readonly HttpClient _http;
    public MenuApiService(HttpClient http) => _http = http;

    public async Task<ApiResultado<List<ProductoMenuDto>>> ListarAsync(string? busqueda, bool incluirInactivos = true)
    {
        var url = $"api/productos-menu?incluirInactivos={incluirInactivos.ToString().ToLower()}";
        if (!string.IsNullOrWhiteSpace(busqueda))
            url += $"&busqueda={Uri.EscapeDataString(busqueda)}";
        return await Leer<List<ProductoMenuDto>>(await _http.GetAsync(url));
    }

    public async Task<ApiResultado<ProductoMenuDetalleDto>> ObtenerAsync(int id) =>
        await Leer<ProductoMenuDetalleDto>(await _http.GetAsync($"api/productos-menu/{id}"));

    public async Task<ApiResultado<ProductoMenuDetalleDto>> CrearAsync(CrearProductoMenuDto dto) =>
        await Leer<ProductoMenuDetalleDto>(await _http.PostAsJsonAsync("api/productos-menu", dto));

    public async Task<ApiResultado<ProductoMenuDetalleDto>> ActualizarPutAsync(int id, ActualizarProductoMenuDto dto) =>
        await Leer<ProductoMenuDetalleDto>(await _http.PutAsJsonAsync($"api/productos-menu/{id}", dto));

    public async Task<ApiResultado<ProductoMenuDetalleDto>> ActualizarPatchAsync(int id, PatchProductoMenuDto dto) =>
        await Leer<ProductoMenuDetalleDto>(await _http.PatchAsJsonAsync($"api/productos-menu/{id}", dto));

    public async Task<ApiResultado<bool>> EliminarAsync(int id)
    {
        var res = await _http.DeleteAsync($"api/productos-menu/{id}");
        return res.IsSuccessStatusCode
            ? new(true, true, null)
            : new(false, false, await LeerError(res));
    }

    public async Task<ApiResultado<CostoProductoDto>> ObtenerCostoAsync(int id) =>
        await Leer<CostoProductoDto>(await _http.GetAsync($"api/productos-menu/{id}/costo"));

    public async Task<ApiResultado<CostoProductoDto>> CostoPreviewAsync(
        List<RecetaEnProductoDto> recetas, List<InsumoDirectoDto> empaques) =>
        await Leer<CostoProductoDto>(await _http.PostAsJsonAsync("api/productos-menu/costo-preview",
            new { recetas, insumosDirectos = empaques }));

    // costosFijosIds vacío = precio con costo directo; null = todos los fijos.
    // La interfaz envía lista vacía por defecto para no cargar todos los fijos
    // al precio de un solo producto.
    public async Task<ApiResultado<PrecioSugeridoProductoDto>> PrecioSugeridoAsync(
        int id, decimal margen, List<int>? costosFijosIds = null) =>
        await Leer<PrecioSugeridoProductoDto>(await _http.PostAsJsonAsync(
            $"api/productos-menu/{id}/precio-sugerido",
            new { margenDeseado = margen, costosFijosIds }));

    public async Task<ApiResultado<PrecioSugeridoProductoDto>> PrecioPreviewAsync(
        List<RecetaEnProductoDto> recetas, List<InsumoDirectoDto> empaques,
        decimal margen, List<int>? costosFijosIds = null) =>
        await Leer<PrecioSugeridoProductoDto>(await _http.PostAsJsonAsync(
            "api/productos-menu/precio-sugerido-preview",
            new { recetas, insumosDirectos = empaques, margenDeseado = margen, costosFijosIds }));

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
