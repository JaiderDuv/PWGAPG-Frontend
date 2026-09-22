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
            var identity = _authService.IsAuthenticated
                ? new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, _authService.NombreEmpresa) }, "apiauth")
                : new ClaimsIdentity();

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }
    }
}