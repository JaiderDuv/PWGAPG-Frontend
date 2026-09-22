using Microsoft.JSInterop;
using PWGAPG_Frontend.Models;
using System.Net.Http.Json;

namespace PWGAPG_Frontend.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _jsRuntime;

        public bool IsAuthenticated { get; private set; } = false;
        public string NombreEmpresa { get; private set; } = string.Empty;
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
            var resp = await _httpClient.PostAsJsonAsync("api/auth/login", dto); // confirma la ruta exacta en tu AuthController
            if (!resp.IsSuccessStatusCode) return null;

            var resultado = await resp.Content.ReadFromJsonAsync<LoginResponseDto>();

            if (resultado is { Exito: true })
            {
                IsAuthenticated = true;
                NombreEmpresa = resultado.NombreEmprendimiento ?? string.Empty;
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", resultado.Token ?? resultado.AccessToken);
                NotifyStateChanged();
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "nombreEmpresa", NombreEmpresa);
            }

            return resultado;
        }

        public async Task InicializarAsync()
        {
            var token = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "authToken");
            if (!string.IsNullOrWhiteSpace(token))
            {
                IsAuthenticated = true;
                NombreEmpresa = await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", "nombreEmpresa") ?? string.Empty;
                NotifyStateChanged();
            }
        }

        public async Task CerrarSesionAsync()
        {
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "nombreEmpresa");

            IsAuthenticated = false;
            NombreEmpresa = string.Empty;
            DescripcionEmpresa = string.Empty;
            UrlFotoPerfil = "images/default-avatar.png";

            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}