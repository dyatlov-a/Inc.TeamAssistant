using System.Numerics;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
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
        using var image = await Image.LoadAsync(Path.Combine(_webRootPath, _options.ImgFolder, img), token);
        
        var totalPadding = _options.Padding * 2;
        var imageWidth = image.Width;
        var imageHeight = image.Height;
        var wrappingLength = imageWidth - totalPadding;
        
        var textArea = TextMeasurer.MeasureAdvance(text, CreateTextOptions(font, wrappingLength));
        var x = imageWidth / 2f - textArea.Width / 2;
        var y = imageHeight / 2f - textArea.Height / 2;
        
        image.Mutate(i => i
            .Fill(
                CreateDrawingOptions(),
                Color.Black,
                new RectangleF(0, y - _options.Padding, imageWidth, textArea.Height + totalPadding))
            .DrawText(
                CreateTextOptions(font, wrappingLength, new Vector2(x, y)),
                text,
                Color.White));

        return await CreateStream(image, token);
    }

    private Font CreateFont()
    {
        if (!SystemFonts.TryGet(_options.FontFamily, out var fontFamily))
            throw new ApplicationException($"Couldn't find font {_options.FontFamily}");

        return fontFamily.CreateFont(_options.FontSize, _options.FontStyle);
    }
    
    private async Task<MemoryStream> CreateStream(Image image, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(image);
        
        var stream = new MemoryStream();
        
        await image.SaveAsJpegAsync(stream, token);
        stream.Position = 0;

        return stream;
    }

    private RichTextOptions CreateTextOptions(Font font, float wrappingLength, Vector2? origin = null)
    {
        ArgumentNullException.ThrowIfNull(font);
        
        var textOptions = new RichTextOptions(font)
        {
            KerningMode = KerningMode.Standard,
            TextAlignment = TextAlignment.Center,
            WrappingLength = wrappingLength,
            LineSpacing = _options.LineSpacing
        };
        
        if (origin.HasValue)
            textOptions.Origin = origin.Value;

        return textOptions;
    }

    private DrawingOptions CreateDrawingOptions()
    {
        return new DrawingOptions
        {
            GraphicsOptions = new GraphicsOptions
            {
                BlendPercentage = _options.BlendPercentage,
                AlphaCompositionMode = PixelAlphaCompositionMode.SrcOver
            }
        };
    }
}