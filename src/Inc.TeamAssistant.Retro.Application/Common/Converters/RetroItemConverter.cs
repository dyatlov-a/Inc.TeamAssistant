using System.Text.RegularExpressions;
using Inc.TeamAssistant.Retro.Domain;
using Inc.TeamAssistant.Retro.Model.Common;

namespace Inc.TeamAssistant.Retro.Application.Common.Converters;

internal static class RetroItemConverter
{
    private static readonly Regex LetterPattern = new(@"\p{L}", RegexOptions.Compiled);
    
    public static RetroItemDto ConvertTo(
        RetroItem item,
        long? currentPersonId = null,
        RetroSessionState? state = null,
        IDictionary<Guid, int>? votesByPerson = null)
    {
        ArgumentNullException.ThrowIfNull(item);

        const int defaultVotes = 0;
        var votes = state switch
        {
            RetroSessionState.Prioritizing => votesByPerson?.TryGetValue(item.Id, out var value) == true ? value : null,
            RetroSessionState.Discussing or RetroSessionState.Finished => item.Votes,
            _ => null
        };
        var parentText = state is null && currentPersonId.HasValue
            ? ToObfuscate(item, currentPersonId.Value)
            : item.Text;
        
        return new RetroItemDto(
            item.Id,
            item.TeamId,
            item.Created,
            item.ColumnId,
            item.Position,
            parentText,
            item.OwnerId,
            item.ParentId,
            votes ?? defaultVotes,
            item.Children.Select(c => new RetroItemDto(
                c.Id,
                c.TeamId,
                c.Created,
                c.ColumnId,
                c.Position,
                c.Text,
                c.OwnerId,
                c.ParentId,
                Votes: defaultVotes,
                Children: [])).ToArray());
    }

    private static string? ToObfuscate(RetroItem item, long currentPersonId)
    {
        ArgumentNullException.ThrowIfNull(item);
        
        const string obfuscatedLetter = "А";
        const string obfuscatedLetterLower = "а";

        var obfuscatedText = currentPersonId == item.OwnerId || string.IsNullOrWhiteSpace(item.Text)
            ? item.Text
            : LetterPattern.Replace(
                item.Text,
                m => char.IsUpper(m.Value[0]) ? obfuscatedLetter : obfuscatedLetterLower);

        return obfuscatedText;
    }
}