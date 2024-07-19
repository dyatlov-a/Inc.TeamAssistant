using System.Text.Json;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.WebUI.Contracts;

namespace Inc.TeamAssistant.WebUI.Extensions;

public static class DataEditorExtensions
{
    public static async Task<T?> Get<T>(
        this IDataEditor dataEditor,
        string key,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(dataEditor);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var data = await dataEditor.Get(key, token);
        if (data.State != ServiceResultState.Success || data.Result is null)
            return default;

        var result = JsonSerializer.Deserialize<T>(data.Result);
        return result;
    }

    public static async Task Attach<T>(
        this IDataEditor dataEditor,
        string key,
        T data,
        CancellationToken token = default)
    {
        ArgumentNullException.ThrowIfNull(dataEditor);
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(data);

        await dataEditor.Attach(key, JsonSerializer.Serialize(data), token);
    }
}