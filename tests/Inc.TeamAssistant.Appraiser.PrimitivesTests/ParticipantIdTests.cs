using AutoFixture;
using Inc.TeamAssistant.Appraiser.Primitives;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.PrimitivesTests;

public sealed class ParticipantIdTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ValidValue_ShouldBeCreated()
    {
        var actual = new ParticipantId(_fixture.Create<long>());

        Assert.NotNull(actual);
    }

    [Fact]
    public void Constructor_NotValidValue_ThrowsException()
    {
        ParticipantId Actual() => new(default);

        Assert.Throws<ArgumentException>(Actual);
    }
}