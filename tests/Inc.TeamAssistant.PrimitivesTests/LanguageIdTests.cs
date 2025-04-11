using AutoFixture;
using Inc.TeamAssistant.Primitives.Languages;
using Xunit;

namespace Inc.TeamAssistant.PrimitivesTests;

public sealed class LanguageIdTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void Constructor_ValidValue_ShouldBeCreated()
    {
        var actual = new LanguageId(_fixture.Create<string>());

        Assert.NotNull(actual);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_NotValidValue_ThrowsException(string? value)
    {
        LanguageId Actual() => new(value!);

        Assert.Throws<ArgumentException>(Actual);
    }
    
    [Fact]
    public void Constructor_NullValue_ThrowsException()
    {
        LanguageId Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }
}