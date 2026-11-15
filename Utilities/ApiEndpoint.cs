namespace PWGAPG_Frontend.Utilities;

public static class ApiEndpoint
{
    public static Uri Resolve(string frontendBaseAddress, string? publicBaseUrl, string? localBaseUrl)
    {
        var isLocal = new Uri(frontendBaseAddress).IsLoopback;
        var url = isLocal && !string.IsNullOrWhiteSpace(localBaseUrl) ? localBaseUrl : publicBaseUrl;
        if (string.IsNullOrWhiteSpace(url))
            url = "https://chefmetrics-backend.onrender.com/";

        if (!Uri.TryCreate(url.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp) ||
            (!isLocal && (uri.IsLoopback || uri.Scheme != Uri.UriSchemeHttps)) ||
            !string.IsNullOrEmpty(uri.Query) || !string.IsNullOrEmpty(uri.Fragment) ||
            !string.IsNullOrEmpty(uri.UserInfo))
        {
            throw new InvalidOperationException("ApiSettings:BaseUrl debe apuntar a la API pública mediante HTTPS al publicar el sitio.");
        }

        return new Uri(uri.AbsoluteUri.TrimEnd('/') + "/");
    }
}
