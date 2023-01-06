using Inc.TeamAssistant.Appraiser.Application.Contracts;
using QRCoder;

namespace Inc.TeamAssistant.Appraiser.Backend.Services;

internal sealed class QuickResponseCodeGenerator : IQuickResponseCodeGenerator
{
    public string Generate(string data, int width, int height, bool drawQuietZones)
    {
        if (string.IsNullOrWhiteSpace(data))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(data));

        var eccLevel = QRCodeGenerator.ECCLevel.Q;

        using var qrGenerator = new QRCodeGenerator();
        using var qrCode = new SvgQRCode(qrGenerator.CreateQrCode(data, eccLevel));

        return qrCode.GetGraphic(new(width, height), drawQuietZones);
    }
}