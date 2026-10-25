using Microsoft.JSInterop;
using PWGAPG_Frontend.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace PWGAPG_Frontend.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;
        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        public bool IsAuthenticated { get; private set; } = false;
        public string NombreEmpresa { get; private set; } = string.Empty;
        public bool OnboardingCompletado { get; private set; } = false;
        public string DescripcionEmpresa { get; private set; } = string.Empty;
        public string UrlFotoPerfil { get; private set; } = "images/default-avatar.png";

        public event Action? OnChange;

        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        public async Task<LoginResponseDto?> Login(LoginDTO dto)
        {
            try
            {
                var resp = await _httpClient.PostAsJsonAsync("api/auth/login", dto);

                LoginResponseDto? resultado = null;
                try
                {
                    resultado = await resp.Content.ReadFromJsonAsync<LoginResponseDto>();
                }
                catch
                {
                    // El cuerpo no es un LoginResponseDto (por ejemplo, un 400 de validación o vacío)
                }

                if (!resp.IsSuccessStatusCode)
                {
                    return new LoginResponseDto
                    {
                        Exito = false,
                        Mensaje = string.IsNullOrWhiteSpace(resultado?.Mensaje)
                            ? "Correo o contraseña incorrectos."
                            : resultado!.Mensaje
                    };
                }

                if (resultado is { Exito: true })
                {
                    await GuardarSesionAsync(resultado);
                }

                return resultado;
            }
            catch (HttpRequestException)
            {
                return new LoginResponseDto
                {
                    Exito = false,
                    Mensaje = "No se pudo conectar con el servidor. Intenta de nuevo más tarde."
                };
            }
        }

        public async Task InicializarAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (string.IsNullOrWhiteSpace(token))
            {
                return;
            }

            IsAuthenticated = true;
            NombreEmpresa = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "nombreEmpresa") ?? string.Empty;

            var onboardingGuardado = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "onboardingCompletado");
            OnboardingCompletado = onboardingGuardado == "True";

            try
            {
                // Refresca el flag real desde BD (sobrescribe el de localStorage).
                // Si el access token expiró, el helper renueva la sesión y reintenta.
                using var resp = await EnviarConTokenAsync(HttpMethod.Get, "api/user");
                if (resp.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return;
                }

                if (resp.IsSuccessStatusCode)
                {
                    using var doc = await JsonDocument.ParseAsync(await resp.Content.ReadAsStreamAsync());
                    OnboardingCompletado = doc.RootElement.TryGetProperty("onboardingCompletado", out var prop) &&
                                           prop.ValueKind == JsonValueKind.True;
                    await _jsRuntime.InvokeVoidAsync(
                        "localStorage.setItem", "onboardingCompletado", OnboardingCompletado.ToString());
                }

                // Refresca la foto de perfil desde BD.
                using var respFoto = await EnviarConTokenAsync(HttpMethod.Get, "api/onboarding/datos-personales");
                if (respFoto.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return;
                }

                if (respFoto.IsSuccessStatusCode)
                {
                    using var docFoto = await JsonDocument.ParseAsync(await respFoto.Content.ReadAsStreamAsync());
                    var foto = docFoto.RootElement.TryGetProperty("fotoPerfilUrl", out var fotoProp) &&
                               fotoProp.ValueKind == JsonValueKind.String
                        ? fotoProp.GetString()
                        : null;

                    UrlFotoPerfil = string.IsNullOrWhiteSpace(foto)
                        ? "images/default-avatar.png"
                        : foto;
                }
            }
            catch (HttpRequestException)
            {
                // Sin conexión: se conserva la sesión local y sus datos.
            }

            NotifyStateChanged();
        }

        public void ActualizarFotoPerfil(string? url)
        {
            UrlFotoPerfil = string.IsNullOrWhiteSpace(url) ? "images/default-avatar.png" : url;
            NotifyStateChanged();
        }

        public async Task<(bool exito, string mensaje)> ReenviarVerificacionAsync(string correo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(correo))
                    return (false, "Ingresa tu correo electrónico primero.");

                var resp = await _httpClient.PostAsJsonAsync(
                    "api/usuarios/reenviar-verificacion",
                    new { Correo = correo.Trim() });

                var cuerpo = await resp.Content.ReadAsStringAsync();
                string mensaje = "Si el correo está registrado y pendiente de verificación, recibirás un nuevo enlace en breve.";

                if (!string.IsNullOrWhiteSpace(cuerpo))
                {
                    try
                    {
                        using var doc = JsonDocument.Parse(cuerpo);
                        if (doc.RootElement.TryGetProperty("mensaje", out var m) && m.ValueKind == JsonValueKind.String)
                            mensaje = m.GetString() ?? mensaje;
                        else if (doc.RootElement.TryGetProperty("Mensaje", out var m2) && m2.ValueKind == JsonValueKind.String)
                            mensaje = m2.GetString() ?? mensaje;
                    }
                    catch { /* si no es JSON, usar mensaje por defecto */ }
                }

                return (resp.IsSuccessStatusCode, mensaje);
            }
            catch (HttpRequestException)
            {
                return (false, "No se pudo conectar con el servidor. Intenta de nuevo más tarde.");
            }
        }

        public async Task<bool> CompletarOnboardingAsync()
        {
            try
            {
                using var resp = await EnviarConTokenAsync(HttpMethod.Post, "api/onboarding/completar");
                if (!resp.IsSuccessStatusCode)
                {
                    return false;
                }

                OnboardingCompletado = true;
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "onboardingCompletado", "True");
                NotifyStateChanged();
                return true;
            }
            catch (HttpRequestException)
            {
                return false;
            }
        }

        public async Task<bool> RenovarSesionAsync()
        {
            await _refreshLock.WaitAsync();
            try
            {
                var refreshToken = await ObtenerRefreshTokenAsync();
                if (string.IsNullOrWhiteSpace(refreshToken))
                {
                    await LimpiarSesionAsync();
                    return false;
                }

                try
                {
                    using var response = await _httpClient.PostAsJsonAsync(
                        "api/auth/refresh", new { Token = refreshToken });

                    if (!response.IsSuccessStatusCode)
                    {
                        await LimpiarSesionAsync();
                        return false;
                    }

                    var session = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (session is not { Exito: true } || string.IsNullOrWhiteSpace(session.AccessToken ?? session.Token))
                    {
                        await LimpiarSesionAsync();
                        return false;
                    }

                    await GuardarSesionAsync(session);
                    return true;
                }
                catch (HttpRequestException)
                {
                    return false;
                }
            }
            finally
            {
                _refreshLock.Release();
            }
        }

        public async Task CerrarSesionAsync()
        {
            var refreshToken = await ObtenerRefreshTokenAsync();
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                try
                {
                    await _httpClient.PostAsJsonAsync("api/auth/logout", new { Token = refreshToken });
                }
                catch (HttpRequestException)
                {
                    // La sesión local se elimina aunque el servidor no esté disponible.
                }
            }

            await LimpiarSesionAsync();
        }

        public async Task<string?> ObtenerAccessTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
        }

        private async Task<string?> ObtenerRefreshTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "refreshToken");
        }

        private async Task<HttpResponseMessage> EnviarConTokenAsync(
            HttpMethod method,
            string url)
        {
            var request = new HttpRequestMessage(method, url);
            await AgregarBearerAsync(request);
            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.Unauthorized)
            {
                return response;
            }

            if (!await RenovarSesionAsync())
            {
                return response;
            }

            response.Dispose();
            var retry = new HttpRequestMessage(method, url);
            await AgregarBearerAsync(retry);
            return await _httpClient.SendAsync(retry);
        }

        private async Task AgregarBearerAsync(HttpRequestMessage request)
        {
            var token = await ObtenerAccessTokenAsync();
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private async Task GuardarSesionAsync(LoginResponseDto resultado)
        {
            var accessToken = resultado.AccessToken ?? resultado.Token;
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return;
            }

            IsAuthenticated = true;
            NombreEmpresa = resultado.NombreEmprendimiento ?? string.Empty;
            OnboardingCompletado = resultado.OnboardingCompletado;

            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", accessToken);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "nombreEmpresa", NombreEmpresa);
            await _jsRuntime.InvokeVoidAsync(
                "localStorage.setItem", "onboardingCompletado", OnboardingCompletado.ToString());

            if (!string.IsNullOrWhiteSpace(resultado.RefreshToken))
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "refreshToken", resultado.RefreshToken);
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
            }

            NotifyStateChanged();
        }

        private async Task LimpiarSesionAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "refreshToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "nombreEmpresa");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "onboardingCompletado");

            IsAuthenticated = false;
            NombreEmpresa = string.Empty;
            DescripcionEmpresa = string.Empty;
            UrlFotoPerfil = "images/default-avatar.png";
            OnboardingCompletado = false;

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
