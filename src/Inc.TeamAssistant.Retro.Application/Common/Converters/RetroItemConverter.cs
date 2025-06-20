using System.Text.RegularExpressions;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Common.Converters;

internal static class RetroItemConverter
{
    private static readonly Regex LetterPattern = new(@"\p{L}", RegexOptions.Compiled);

    public static RetroItemDto ConvertFromCreated(RetroItem item, RetroTypes retroType)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        return ConvertFrom(item, currentPersonId: null, state: null, votesByPerson: null, retroType);
    }
    
    public static RetroItemDto ConvertFromChanged(RetroItem item, RetroSessionState? state, RetroTypes retroType)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        return ConvertFrom(item, currentPersonId: null, state, votesByPerson: null, retroType);
    }

    public static RetroItemDto ConvertFromReadModel(
        RetroItem item,
        long currentPersonId,
        RetroSessionState? state,
        IDictionary<Guid, int> votesByPerson,
        RetroTypes retroType)
    {
        ArgumentNullException.ThrowIfNull(item);

        return ConvertFrom(item, currentPersonId, state, votesByPerson, retroType);
    }
    
    private static RetroItemDto ConvertFrom(
        RetroItem item,
        long? currentPersonId,
        RetroSessionState? state,
        IDictionary<Guid, int>? votesByPerson,
        RetroTypes retroType)
    {
        ArgumentNullException.ThrowIfNull(item);

        const int defaultVotes = 0;
        var votes = state switch
        {
            RetroSessionState.Prioritizing => votesByPerson?.TryGetValue(item.Id, out var value) == true ? value : null,
            RetroSessionState.Discussing or RetroSessionState.Finished => item.Votes,
            _ => null
        };
        var parentText = state is null && retroType == RetroTypes.Hidden
            ? ToObfuscate(item, currentPersonId)
            : item.Text;
        
        return new RetroItemDto(
            item.Id,
            item.RoomId,
            item.ColumnId,
            item.Position,
            parentText,
            item.OwnerId,
            item.ParentId,
            votes ?? defaultVotes);
    }

    private static string? ToObfuscate(RetroItem item, long? currentPersonId)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (string.IsNullOrWhiteSpace(item.Text))
            return null;
        
        const string obfuscatedLetter = "А";
        const string obfuscatedLetterLower = "а";

        var obfuscatedText = currentPersonId == item.OwnerId
            ? item.Text
            : LetterPattern.Replace(
                item.Text,
                m => char.IsUpper(m.Value[0]) ? obfuscatedLetter : obfuscatedLetterLower);

        return obfuscatedText;
    }
}