using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace PWGAPG_Frontend.Services
{
    public class AuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public bool IsAuthenticated { get; private set; } = false;
        public string NombreEmpresa { get; private set; } = string.Empty;
        public string DescripcionEmpresa { get; private set; } = string.Empty;
        public string UrlFotoPerfil { get; private set; } = "images/default-avatar.png";

        public event Action? OnChange;

        // Inyección de HttpClient para consultar tu API y JSInterop para localStorage
        public AuthService(HttpClient httpClient, IJSRuntime jsRuntime)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
        }

        // Método de Inicio de Sesión asíncrono para conectar con la API
        public async Task<bool> IniciarSesionAsync(string correo, string password)
        {
            try
            {
                // TODO: Descomentar al tener el endpoint real de C# / Web API
                // var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { correo, password });
                // if (!response.IsSuccessStatusCode) return false;
                // var resultado = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                // Guardar token JWT o indicador de sesión en localStorage
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", "TOKEN_RECIBIDO_DEL_BACKEND");

                IsAuthenticated = true;
                NombreEmpresa = "Empresa Ejemplo"; // Reemplazar por resultado.NombreEmpresa
                DescripcionEmpresa = "Descripción recuperada de la API.";

                NotifyStateChanged();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Método de Cierre de Sesión real que remueve el token almacenado
        public async Task CerrarSesionAsync()
        {
            // 1. Elimina el token guardado en el navegador
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");

            // 2. Opcional: Opcionalmente puedes notificar a tu backend sobre el logout
            // await _httpClient.PostAsync("api/auth/logout", null);

            // 3. Limpia el estado interno en memoria
            IsAuthenticated = false;
            NombreEmpresa = string.Empty;
            DescripcionEmpresa = string.Empty;
            UrlFotoPerfil = "images/default-avatar.png";

            // 4. Notifica a la interfaz para refrescar vistas
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}