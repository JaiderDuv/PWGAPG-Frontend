using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PWGAPG_Frontend;
using PWGAPG_Frontend.Services;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = builder.Configuration["ApiSettings:BaseUrl"]!;
builder.Services.AddHttpClient<IAuthService, AuthService>(c => c.BaseAddress = new Uri(apiUrl));
builder.Services.AddScoped<AuthService>(sp => (AuthService)sp.GetRequiredService<IAuthService>());
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiUrl) });

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

var host = builder.Build();

// Restaura la sesión (si hay token guardado) ANTES de mostrar cualquier página
var authService = host.Services.GetRequiredService<AuthService>();
await authService.InicializarAsync();

await host.RunAsync();
