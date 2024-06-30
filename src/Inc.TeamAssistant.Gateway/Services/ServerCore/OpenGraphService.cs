using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace Inc.TeamAssistant.Gateway.Services.ServerCore;

public sealed class OpenGraphService
{
    private readonly string _webRootPath;

    public OpenGraphService(string webRootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(webRootPath);
        
        _webRootPath = webRootPath;
    }

    public async Task<MemoryStream> CreateCard(
        string img,
        string text,
        string textFont,
        float fontSize,
        FontStyle fontStyle,
        int padding,
        CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(img);
        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        ArgumentException.ThrowIfNullOrWhiteSpace(textFont);
        
        var font = CreateFont(textFont, fontSize, fontStyle);
        using var image = await Image.LoadAsync(GetFileName(img), token);
        
        var imageWidth = image.Width;
        var imageHeight = image.Height;
        var options = new TextOptions(font)
        {
            KerningMode = KerningMode.Standard,
            TextAlignment = TextAlignment.Center,
            WrappingLength = imageWidth - padding * 2
        };
        var textArea = TextMeasurer.MeasureAdvance(text, options);
        var x = imageWidth / 2f - textArea.Width / 2;
        var y = imageHeight / 2f - textArea.Height / 2;

        image.Mutate(i => i
            .Fill(Color.Black, new RectangleF(0, y - padding, imageWidth, textArea.Height + padding * 2))
            .DrawText(text, font, Color.White, new PointF(x, y)));

        return await CreateStream(image, token);
    }

    private static Font CreateFont(string textFont, float fontSize, FontStyle fontStyle)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(textFont);
        
        if (!SystemFonts.TryGet(textFont, out FontFamily fontFamily))
            throw new Exception($"Couldn't find font {textFont}");

        return fontFamily.CreateFont(fontSize, fontStyle);
    }
    
    private string GetFileName(string img)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(img);
        
        return Path.Combine(_webRootPath, "og", img);
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