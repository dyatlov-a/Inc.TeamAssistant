using AutoFixture;
using Inc.TeamAssistant.Appraiser.Primitives;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.PrimitivesTests;

public sealed class AssessmentSessionIdTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ValidValue_ShouldBeCreated()
    {
        var actual = new AssessmentSessionId(_fixture.Create<Guid>());

        Assert.NotNull(actual);
    }

    [Fact]
    public void Constructor_NotValidValue_ThrowsException()
    {
        AssessmentSessionId Actual() => new(Guid.Empty);

        Assert.Throws<ArgumentException>(Actual);
    }
}