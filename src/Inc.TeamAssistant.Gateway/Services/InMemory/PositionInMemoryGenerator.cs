using Inc.TeamAssistant.Retro.Application.Contracts;

namespace Inc.TeamAssistant.Gateway.Services.InMemory;

internal sealed class PositionInMemoryGenerator : IPositionGenerator
{
    private int _position;
    
    public int Generate()
    {
        Interlocked.Increment(ref _position);

        return _position;
    }
}