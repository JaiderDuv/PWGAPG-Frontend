using System.Net.Http.Json;
using System.Text.Json;
using PWGAPG_Frontend.Models;

namespace PWGAPG_Frontend.Services;

public sealed class EmpleadoApiService
{
    private readonly HttpClient _http;

    public EmpleadoApiService(HttpClient http) => _http = http;

    public async Task<ApiResultado<EmpleadoDto>> InvitarAsync(InvitarEmpleadoDto dto)
    {
        using var response = await _http.PostAsJsonAsync("api/empleados/invitaciones", dto);
        return await LeerAsync<EmpleadoDto>(response);
    }

    public async Task<ApiResultado<EmpleadoDto>> ActualizarAsync(int id, ActualizarEmpleadoDto dto)
    {
        using var response = await _http.PutAsJsonAsync($"api/empleados/{id}", dto);
        return await LeerAsync<EmpleadoDto>(response);
    }

    public async Task<ApiResultado<bool>> CambiarEstadoAsync(int id, bool activo)
    {
        using var response = await _http.PatchAsJsonAsync(
            $"api/empleados/{id}/estado", new ActualizarEstadoEmpleadoDto { Activo = activo });
        if (!response.IsSuccessStatusCode)
            return new(false, false, await LeerMensajeAsync(response));
        return new(true, true, null);
    }

    public async Task<ApiResultado<bool>> EliminarAsync(int id)
    {
        using var response = await _http.DeleteAsync($"api/empleados/{id}");
        if (!response.IsSuccessStatusCode)
            return new(false, false, await LeerMensajeAsync(response));
        return new(true, true, null);
    }

    public async Task<ApiResultado<AceptarInvitacionRespuestaDto>> AceptarInvitacionAsync(string token)
    {
        using var response = await _http.PostAsJsonAsync(
            "api/empleados/aceptar-invitacion", new AceptarInvitacionDto { Token = token });
        return await LeerAsync<AceptarInvitacionRespuestaDto>(response);
    }

    private static async Task<ApiResultado<T>> LeerAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
            return new(false, default, await LeerMensajeAsync(response));
        return new(true, await response.Content.ReadFromJsonAsync<T>(), null);
    }

    private static async Task<string> LeerMensajeAsync(HttpResponseMessage response)
    {
        var body = await response.Content.ReadAsStringAsync();
        try
        {
            using var json = JsonDocument.Parse(body);
            if (json.RootElement.TryGetProperty("mensaje", out var message))
                return message.GetString() ?? "No se pudo completar la operación.";
            if (json.RootElement.TryGetProperty("title", out var title))
                return title.GetString() ?? "Revisa los datos enviados.";
        }
        catch (JsonException) { }
        return "No se pudo completar la operación.";
    }
}
