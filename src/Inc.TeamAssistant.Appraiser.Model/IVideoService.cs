namespace Inc.TeamAssistant.Appraiser.Model;

public interface IVideoService
{
    bool IsServer { get; }

    Task Play(string id);
}