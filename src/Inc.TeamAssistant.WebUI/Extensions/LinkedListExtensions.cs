namespace Inc.TeamAssistant.WebUI.Extensions;

public static class LinkedListExtensions
{
    public static LinkedListNode<T>? Find<T>(this LinkedList<T> items, Func<T, bool> selector)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(selector);
        
        var current = items.First;
        
        while (current is not null)
        {
            if (selector(current.Value))
                return current;
            
            current = current.Next;
        }
        
        return null;
    }
}