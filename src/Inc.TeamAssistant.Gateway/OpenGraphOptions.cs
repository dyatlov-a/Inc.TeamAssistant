using SixLabors.Fonts;

namespace Inc.TeamAssistant.Gateway;

public sealed class OpenGraphOptions
{
    public string FontFamily { get; set; } = default!;
    public FontStyle FontStyle { get; set; }
    public float FontSize { get; set; }
    public float LineSpacing { get; set; }
    public int Padding { get; set; }
    public string ImgFolder { get; set; } = default!;
    public float BlendPercentage { get; set; }
}