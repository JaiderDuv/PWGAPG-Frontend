using System.Net;
using System.Text.Json;

namespace PWGAPG_Frontend.Services;

internal static class ErrorRespuestaApi
{
    public static async Task<string> LeerAsync(HttpResponseMessage respuesta)
    {
        if (respuesta.StatusCode == HttpStatusCode.Unauthorized)
            return "Tu sesión expiró. Inicia sesión de nuevo.";

        const string mensajeGenerico = "Ocurrió un error inesperado.";
        try
        {
            using var documento = JsonDocument.Parse(await respuesta.Content.ReadAsStringAsync());
            var raiz = documento.RootElement;
            if (raiz.ValueKind != JsonValueKind.Object)
                return mensajeGenerico;

            if (raiz.TryGetProperty("mensaje", out var mensaje) &&
                mensaje.ValueKind is JsonValueKind.String or JsonValueKind.Null)
                return mensaje.GetString() ?? "Error.";

            if (raiz.TryGetProperty("errors", out var errores) && errores.ValueKind == JsonValueKind.Object)
            {
                var mensajes = errores.EnumerateObject()
                    .Where(campo => campo.Value.ValueKind == JsonValueKind.Array)
                    .SelectMany(campo => campo.Value.EnumerateArray())
                    .Where(valor => valor.ValueKind == JsonValueKind.String)
                    .Select(valor => valor.GetString())
                    .Where(valor => !string.IsNullOrWhiteSpace(valor))
                    .ToArray();

                if (mensajes.Length > 0)
                    return string.Join(" ", mensajes);
            }
        }
        catch (Exception ex) when (ex is JsonException or HttpRequestException or IOException)
        {
            return mensajeGenerico;
        }

        return mensajeGenerico;
    }
}
