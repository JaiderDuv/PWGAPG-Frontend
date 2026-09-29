using System.Net;
using System.Net.Http.Headers;

namespace PWGAPG_Frontend.Services;

public sealed class BearerAuthorizationHandler : DelegatingHandler
{
    private readonly AuthService _authService;

    public BearerAuthorizationHandler(AuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await AddBearerAsync(request);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode != HttpStatusCode.Unauthorized ||
            request.RequestUri?.AbsolutePath.Contains("/api/auth/", StringComparison.OrdinalIgnoreCase) == true ||
            request.RequestUri?.AbsolutePath.Contains("/api/perfil/confirmar-identidad", StringComparison.OrdinalIgnoreCase) == true ||
            !await _authService.RenovarSesionAsync())
        {
            return response;
        }

        response.Dispose();
        var retry = await CloneRequestAsync(request, cancellationToken);
        await AddBearerAsync(retry);
        return await base.SendAsync(retry, cancellationToken);
    }

    private async Task AddBearerAsync(HttpRequestMessage request)
    {
        var accessToken = await _authService.ObtenerAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri)
        {
            Version = request.Version,
            VersionPolicy = request.VersionPolicy
        };

        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (request.Content is not null)
        {
            var content = await request.Content.ReadAsByteArrayAsync(cancellationToken);
            clone.Content = new ByteArrayContent(content);

            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        return clone;
    }
}
