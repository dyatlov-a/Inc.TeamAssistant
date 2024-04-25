using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Net.Codecrete.QrCodeGenerator;

namespace Inc.TeamAssistant.Gateway.Services.Internal;

internal sealed class QuickResponseCodeGenerator : IQuickResponseCodeGenerator
{
    public string Generate(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(data));
        
        var qr = QrCode.EncodeText(data, QrCode.Ecc.Quartile);
        return qr.ToSvgString(border: 0);
    }
}