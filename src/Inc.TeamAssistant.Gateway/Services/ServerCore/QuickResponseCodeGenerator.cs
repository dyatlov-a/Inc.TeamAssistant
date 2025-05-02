using Inc.TeamAssistant.Primitives;
using Net.Codecrete.QrCodeGenerator;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class QuickResponseCodeGenerator : IQuickResponseCodeGenerator
{
    public Task<string> Generate(string data, string foreground, string background, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(foreground);
        ArgumentException.ThrowIfNullOrWhiteSpace(background);

        const int border = 0;
        var qr = QrCode.EncodeText(data, QrCode.Ecc.Quartile);
        return Task.FromResult(qr.ToSvgString(border, $"#{foreground}", $"#{background}"));
    }
}