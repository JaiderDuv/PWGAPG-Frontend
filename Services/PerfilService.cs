using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PWGAPG_Frontend.Models;

namespace PWGAPG_Frontend.Services;

public sealed class PerfilService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public PerfilService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PerfilApiResult<PermisoEdicionPerfilDto>> ConfirmarIdentidadAsync(
        ConfirmarEdicionPerfilDto request)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/perfil/confirmar-identidad", request);

        return await CrearResultadoAsync<PermisoEdicionPerfilDto>(response);
    }

    public async Task<PerfilApiResult<PerfilEmprendedorDto>> ObtenerPerfilAsync(
        string editToken)
    {
        using var request = CrearRequest(HttpMethod.Get, "api/perfil", editToken);
        using var response = await _httpClient.SendAsync(request);

        return await CrearResultadoAsync<PerfilEmprendedorDto>(response);
    }

    public async Task<PerfilApiResult<ActualizarPerfilResultadoDto>> ActualizarPerfilAsync(
        PerfilEmprendedorDto perfil,
        string editToken)
    {
        using var request = CrearRequest(HttpMethod.Put, "api/perfil", editToken);
        request.Content = JsonContent.Create(perfil);

        using var response = await _httpClient.SendAsync(request);
        return await CrearResultadoAsync<ActualizarPerfilResultadoDto>(response);
    }

    // Perfil de usuario: libre, sin token de edición. Solo JWT (Bearer handler).
    public async Task<PerfilApiResult<PerfilUsuarioDto>> ObtenerPerfilUsuarioAsync()
    {
        using var response = await _httpClient.GetAsync("api/perfil/usuario");
        return await CrearResultadoAsync<PerfilUsuarioDto>(response);
    }

    public async Task<PerfilApiResult<ActualizarPerfilResultadoDto>> ActualizarPerfilUsuarioAsync(
        PerfilUsuarioDto perfil)
    {
        using var response = await _httpClient.PutAsJsonAsync("api/perfil/usuario", perfil);
        return await CrearResultadoAsync<ActualizarPerfilResultadoDto>(response);
    }

    // Catálogo de roles para el desplegable de empleados.
    public async Task<PerfilApiResult<List<RolDto>>> ObtenerRolesAsync()
    {
        using var response = await _httpClient.GetAsync("api/perfil/roles");
        return await CrearResultadoAsync<List<RolDto>>(response);
    }

    private static HttpRequestMessage CrearRequest(
        HttpMethod method,
        string url,
        string editToken)
    {
        var request = new HttpRequestMessage(method, url);
        request.Headers.TryAddWithoutValidation("X-Profile-Edit-Token", editToken);
        return request;
    }

    private static async Task<PerfilApiResult<T>> CrearResultadoAsync<T>(
        HttpResponseMessage response)
    {
        var body = response.Content is null
            ? string.Empty
            : await response.Content.ReadAsStringAsync();
        var message = ExtraerMensaje(body);

        if (!response.IsSuccessStatusCode)
        {
            return new PerfilApiResult<T>(
                false,
                default,
                response.StatusCode,
                message);
        }

        var data = string.IsNullOrWhiteSpace(body)
            ? default
            : JsonSerializer.Deserialize<T>(body, JsonOptions);
        return new PerfilApiResult<T>(true, data, response.StatusCode, message);
    }

    private static string? ExtraerMensaje(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("mensaje", out var mensaje))
            {
                return mensaje.GetString();
            }

            if (document.RootElement.TryGetProperty("message", out var message))
            {
                return message.GetString();
            }
        }
        catch (JsonException)
        {
            return body;
        }

        return null;
    }
}

public sealed record PerfilApiResult<T>(
    bool Success,
    T? Data,
    HttpStatusCode StatusCode,
    string? Message);
