using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PWGAPG_Frontend;
using PWGAPG_Frontend.Services;
using PWGAPG_Frontend.Utilities;
using Microsoft.JSInterop;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiUrl = ApiEndpoint.Resolve(
    builder.HostEnvironment.BaseAddress,
    builder.Configuration["ApiSettings:BaseUrl"],
    builder.Configuration["ApiSettings:LocalBaseUrl"]);

builder.Services.AddHttpClient("Auth", c => c.BaseAddress = apiUrl);
// En WebAssembly el host pertenece a una sola pestaña. Todos los clientes HTTP
// comparten la sesión y el bloqueo que evita renovar el mismo token varias veces.
builder.Services.AddSingleton<AuthService>(sp => new AuthService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("Auth"),
    sp.GetRequiredService<IJSRuntime>()));
builder.Services.AddSingleton<IAuthService>(sp => sp.GetRequiredService<AuthService>());
builder.Services.AddScoped<PerfilService>();
builder.Services.AddScoped<EmpleadoApiService>();
builder.Services.AddScoped<InsumoService>();
builder.Services.AddScoped<RecetaApiService>();
builder.Services.AddScoped<CosteoApiService>();
builder.Services.AddScoped<MenuApiService>();
builder.Services.AddScoped<EvaluacionMenuApiService>();
builder.Services.AddScoped<SimuladorVentasApiService>();
builder.Services.AddTransient<BearerAuthorizationHandler>();
builder.Services.AddHttpClient("Api", c => c.BaseAddress = apiUrl)
    .AddHttpMessageHandler<BearerAuthorizationHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

var host = builder.Build();

// Recupera el estado local antes de renderizar. MainLayout lo actualiza con la API
// después del primer render para que un arranque en frío no bloquee toda la interfaz.
var authService = host.Services.GetRequiredService<AuthService>();
await authService.InicializarAsync();

await host.RunAsync();
