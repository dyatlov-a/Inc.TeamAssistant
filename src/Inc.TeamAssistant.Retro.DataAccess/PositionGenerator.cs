using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Retro.DataAccess;

internal sealed class PositionGenerator : IPositionGenerator
{
    private int _position;
    
    public int Generate()
    {
        Interlocked.Increment(ref _position);

        return _position;
    }
}