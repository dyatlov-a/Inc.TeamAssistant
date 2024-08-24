namespace Inc.TeamAssistant.CheckIn.Geo;

internal sealed class RegionLoader
{
    private readonly string _webRootPath;

    public RegionLoader(string webRootPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(webRootPath);
        
        _webRootPath = webRootPath;
    }

    public IEnumerable<string> LoadFile() => File.ReadLines(Path.Combine(_webRootPath, "data", "region-list.json"));
}