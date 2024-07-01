using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

public sealed class OpenGraphService
{
    private readonly OpenGraphOptions _options;
    private readonly string _webRootPath;

    public OpenGraphService(OpenGraphOptions options, string webRootPath)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentException.ThrowIfNullOrWhiteSpace(webRootPath);

        _options = options;
        _webRootPath = webRootPath;
    }

    public async Task<MemoryStream> CreateCard(string img, string text, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(img);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        
        var font = CreateFont();
        using var image = await Image.LoadAsync(GetFileName(img), token);
        
        var totalPadding = _options.Padding * 2;
        var imageWidth = image.Width;
        var imageHeight = image.Height;
        var options = new TextOptions(font)
        {
            KerningMode = KerningMode.Standard,
            TextAlignment = TextAlignment.Center,
            WrappingLength = imageWidth - totalPadding
        };
        var textArea = TextMeasurer.MeasureAdvance(text, options);
        var x = imageWidth / 2f - textArea.Width / 2;
        var y = imageHeight / 2f - textArea.Height / 2;

        image.Mutate(i => i
            .Fill(Color.Black, new RectangleF(0, y - _options.Padding, imageWidth, textArea.Height + totalPadding))
            .DrawText(text, font, Color.White, new PointF(x, y)));

        return await CreateStream(image, token);
    }

    private Font CreateFont()
    {
        if (!SystemFonts.TryGet(_options.FontFamily, out var fontFamily))
            throw new ApplicationException($"Couldn't find font {_options.FontFamily}");

        return fontFamily.CreateFont(_options.FontSize, _options.FontStyle);
    }
    
    private string GetFileName(string img)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(img);
        
        return Path.Combine(_webRootPath, _options.ImgFolder, img);
    }
    
    private async Task<MemoryStream> CreateStream(Image image, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(image);
        
        var stream = new MemoryStream();
        
        await image.SaveAsJpegAsync(stream, token);
        stream.Position = 0;

        return stream;
    }
}