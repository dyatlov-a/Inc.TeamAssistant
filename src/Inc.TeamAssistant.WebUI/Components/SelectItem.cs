namespace Inc.TeamAssistant.WebUI.Components;

public sealed class SelectItem<T>
{
    public string Title { get; set; }
    public T Value { get; set; }

    public SelectItem(string title, T value)
    {
        Title = title;
        Value = value;
    }
}