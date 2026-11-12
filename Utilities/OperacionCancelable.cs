namespace PWGAPG_Frontend.Utilities;

// El componente propietario debe usar esta operación desde el contexto del renderizador.
internal sealed class OperacionCancelable : IAsyncDisposable
{
    private CancellationTokenSource? actual;
    private bool liberada;

    public async ValueTask<CancellationToken> ReiniciarAsync()
    {
        if (liberada)
            return new CancellationToken(canceled: true);

        using var anterior = actual;
        actual = new CancellationTokenSource();
        var token = actual.Token;

        if (anterior is not null)
            await anterior.CancelAsync();

        return token;
    }

    public async ValueTask CancelarAsync()
    {
        using var anterior = actual;
        actual = null;

        if (anterior is not null)
            await anterior.CancelAsync();
    }

    public ValueTask DisposeAsync()
    {
        liberada = true;
        return CancelarAsync();
    }
}
