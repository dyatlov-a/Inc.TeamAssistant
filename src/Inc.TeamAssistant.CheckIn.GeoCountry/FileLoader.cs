using System.Reflection;

namespace Inc.TeamAssistant.CheckIn.GeoCountry;

internal sealed class FileLoader
{
    public IEnumerable<string> LoadFile()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var fileName = assembly.GetManifestResourceNames().Single(f => f.EndsWith("region-list.json"));
        
        using var stream = assembly.GetManifestResourceStream(fileName);
        using var reader = new StreamReader(stream ?? throw new InvalidOperationException());

        while (reader.ReadLine() is { } line)
            yield return line;
    }
}