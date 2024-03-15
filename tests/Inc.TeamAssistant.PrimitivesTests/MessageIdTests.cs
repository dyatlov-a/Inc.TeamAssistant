using AutoFixture;
using Inc.TeamAssistant.Primitives;
using Xunit;

namespace Inc.TeamAssistant.PrimitivesTests;

public sealed class MessageIdTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ValidValue_ShouldBeCreated()
    {
        var actual = new MessageId(_fixture.Create<string>());

        Assert.NotNull(actual);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_NotValidValue_ThrowsException(string? value)
    {
        MessageId Actual() => new(value!);

        Assert.Throws<ArgumentException>(Actual);
    }
}