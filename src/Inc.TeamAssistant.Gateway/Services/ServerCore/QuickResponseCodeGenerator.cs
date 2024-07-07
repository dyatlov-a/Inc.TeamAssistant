using Inc.TeamAssistant.Primitives;
using Net.Codecrete.QrCodeGenerator;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

internal sealed class QuickResponseCodeGenerator : IQuickResponseCodeGenerator
{
    public string Generate(string data, string foreground, string background)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(data));
        if (string.IsNullOrWhiteSpace(foreground))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(foreground));
        if (string.IsNullOrWhiteSpace(background))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(background));

        const int border = 0;
        var qr = QrCode.EncodeText(data, QrCode.Ecc.Quartile);
        return qr.ToSvgString(border, $"#{foreground}", $"#{background}");
    }
}