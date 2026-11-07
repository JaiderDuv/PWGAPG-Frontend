using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace PWGAPG_Frontend.Services
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly AuthService _authService;

        public CustomAuthStateProvider(AuthService authService)
        {
            _authService = authService;
            _authService.OnChange += () => NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var claims = new List<Claim>();
            if (_authService.IsAuthenticated)
            {
                claims.Add(new Claim(ClaimTypes.Name, _authService.NombreEmpresa));
                if (!string.IsNullOrWhiteSpace(_authService.Rol))
                    claims.Add(new Claim(ClaimTypes.Role, _authService.Rol));
                claims.Add(new Claim("tipoCuenta", _authService.TipoCuenta));
                if (_authService.NegocioId.HasValue)
                    claims.Add(new Claim("negocioId", _authService.NegocioId.Value.ToString()));
            }
            var identity = _authService.IsAuthenticated
                ? new ClaimsIdentity(claims, "apiauth")
                : new ClaimsIdentity();

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
    }
}
