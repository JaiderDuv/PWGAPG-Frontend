using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using PWGAPG_Frontend.Models;

namespace PWGAPG_Frontend.Services;

public sealed class InsumoService
{
    private readonly HttpClient _httpClient;
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public InsumoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<InsumoApiResult<ListaPaginadaDto<InsumoApiDto>>> ListarAsync(
        string? busqueda,
        string ordenarPor = "nombre",
        string direccion = "asc",
        string? unidadMedida = null,
        string? categoria = null,
        bool incluirInactivos = true,
        int pagina = 1,
        int tamanoPagina = 50)
    {
        var url = $"api/insumos?ordenarPor={Uri.EscapeDataString(ordenarPor)}" +
            $"&direccion={Uri.EscapeDataString(direccion)}" +
            $"&incluirInactivos={(incluirInactivos ? "true" : "false")}" +
            $"&pagina={pagina}&tamanoPagina={tamanoPagina}";

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            url += $"&busqueda={Uri.EscapeDataString(busqueda.Trim())}";
        }

        if (!string.IsNullOrWhiteSpace(categoria))
        {
            url += $"&categoria={Uri.EscapeDataString(categoria.Trim())}";
        }

        using var response = await _httpClient.GetAsync(url);
        var resultado = await CrearResultadoAsync<ListaPaginadaDto<InsumoApiDto>>(response);

        // Filtro por unidad de medida en cliente: el BackEnd no lo ofrece
        // como parámetro y así se evita pedir un cambio en la API.
        if (resultado.Success && resultado.Data is not null &&
            !string.IsNullOrWhiteSpace(unidadMedida))
        {
            var unidad = unidadMedida.Trim();
            resultado.Data.Datos = resultado.Data.Datos
                .Where(i => string.Equals(i.UnidadMedida, unidad, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return resultado;
    }

    public async Task<InsumoApiResult<InsumoApiDto>> CrearAsync(CrearInsumoRequest request)
    {
        using var response = await _httpClient.PostAsJsonAsync(
            "api/insumos", request, JsonOptions);
        return await CrearResultadoAsync<InsumoApiDto>(response);
    }

    public async Task<InsumoApiResult<InsumoApiDto>> ActualizarParcialAsync(
        int id, ActualizarInsumoRequest request)
    {
        using var message = new HttpRequestMessage(HttpMethod.Patch, $"api/insumos/{id}")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        using var response = await _httpClient.SendAsync(message);
        return await CrearResultadoAsync<InsumoApiDto>(response);
    }

    public async Task<InsumoApiResult<EliminarInsumoInfo>> EliminarAsync(int id)
    {
        using var response = await _httpClient.DeleteAsync($"api/insumos/{id}");

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            var body = await response.Content.ReadAsStringAsync();
            return new InsumoApiResult<EliminarInsumoInfo>(
                false,
                null,
                response.StatusCode,
                ExtraerMensaje(body) ?? "El insumo está en uso y no se puede eliminar.",
                ExtraerNumeroRecetas(body));
        }

        return await CrearResultadoAsync<EliminarInsumoInfo>(response);
    }

    public async Task<InsumoApiResult<List<ProveedorDTO>>> ListarProveedoresNegocioAsync()
    {
        using var response = await _httpClient.GetAsync("api/negocios/mio/proveedores");
        return await CrearResultadoAsync<List<ProveedorDTO>>(response);
    }

    public async Task<InsumoApiResult<List<ProveedorDTO>>> BuscarProveedoresAsync(string termino, int limite = 8)
    {
        var url = $"api/negocios/mio/proveedores?busqueda={Uri.EscapeDataString(termino.Trim())}&limite={limite}";
        using var response = await _httpClient.GetAsync(url);
        return await CrearResultadoAsync<List<ProveedorDTO>>(response);
    }

    public async Task<InsumoApiResult<HistorialPreciosDto>> ObtenerHistorialPreciosAsync(int id)
    {
        using var response = await _httpClient.GetAsync($"api/insumos/{id}/historial-precios");
        return await CrearResultadoAsync<HistorialPreciosDto>(response);
    }

    public async Task<InsumoApiResult<HistorialGeneralDto>> ObtenerHistorialGeneralAsync(
        string? nombre, string? proveedor)
    {
        var url = "api/insumos/historial-precios";
        var query = new List<string>();
        if (!string.IsNullOrWhiteSpace(nombre))
        {
            query.Add($"nombre={Uri.EscapeDataString(nombre.Trim())}");
        }
        if (!string.IsNullOrWhiteSpace(proveedor))
        {
            query.Add($"proveedor={Uri.EscapeDataString(proveedor.Trim())}");
        }
        if (query.Count > 0)
        {
            url += "?" + string.Join("&", query);
        }

        using var response = await _httpClient.GetAsync(url);
        return await CrearResultadoAsync<HistorialGeneralDto>(response);
    }

    private static async Task<InsumoApiResult<T>> CrearResultadoAsync<T>(
        HttpResponseMessage response)
    {
        var body = response.Content is null
            ? string.Empty
            : await response.Content.ReadAsStringAsync();
        var message = ExtraerMensaje(body);

        if (!response.IsSuccessStatusCode)
        {
            return new InsumoApiResult<T>(
                false,
                default,
                response.StatusCode,
                message);
        }

        var data = string.IsNullOrWhiteSpace(body)
            ? default
            : JsonSerializer.Deserialize<T>(body, JsonOptions);
        return new InsumoApiResult<T>(true, data, response.StatusCode, message);
    }

    private static string? ExtraerMensaje(string body)
    {
        if (string.IsNullOrWhiteSpace(body))
        {
            return null;
        }

        // Si no es JSON (ej. página HTML de error del servidor), no se muestra
        // el cuerpo crudo en la interfaz: se usa el mensaje por defecto.
        var recortado = body.TrimStart();
        if (!recortado.StartsWith("{") && !recortado.StartsWith("["))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(body);
            // Las listas (ej. proveedores) son arrays top-level: no tienen "mensaje".
            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                return null;
            }
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
            return null;
        }
        catch (InvalidOperationException)
        {
            return null;
        }

        return null;
    }

    private static int ExtraerNumeroRecetas(string body)
    {
        try
        {
            using var document = JsonDocument.Parse(body);
            if (document.RootElement.TryGetProperty("numeroRecetas", out var numero))
            {
                return numero.GetInt32();
            }
        }
        catch (JsonException)
        {
        }

        return 0;
    }
}

public sealed record InsumoApiResult<T>(
    bool Success,
    T? Data,
    HttpStatusCode StatusCode,
    string? Message,
    int NumeroRecetas = 0);

// Respuesta del DELETE cuando es exitoso: { mensaje }.
public sealed class EliminarInsumoInfo
{
    public string Mensaje { get; set; } = string.Empty;
}
