using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Stories.Features;

internal sealed record StubPerson(long Id, string Name, string? Username)
    : Person(Id, Name, Username)
{
    public override string AvatarUrl => "/imgs/user_stub.jpg";
}