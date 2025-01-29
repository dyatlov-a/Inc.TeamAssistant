using Inc.TeamAssistant.Connector.Application;
using Inc.TeamAssistant.Connector.Application.Services;
using Xunit;

namespace Inc.TeamAssistant.Connector.ApplicationTests.Services;

public sealed class AliasServiceTests
{
    private readonly AliasService _target = new(CommandList.BuildAliasMap());
    
    [Fact]
    public void Constructor_AliasMapIsNull_ThrowsException()
    {
        AliasService Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }
    
    [Fact]
    public void Constructor_AliasMapWithDuplicate_ThrowsException()
    {
        AliasService Actual() => new([("/c1", "/command1"), ("/c1", "/command2")]);

        Assert.Throws<ArgumentException>(Actual);
    }

    [Fact]
    public void OverrideCommand_TextIsNull_ThrowsException()
    {
        string Actual() => _target.OverrideCommand(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData("text1", "text1")]
    [InlineData("/nr", "/need_review")]
    [InlineData("/nr1", "/nr1")]
    [InlineData("/nr 1", "/need_review 1")]
    [InlineData("/l", "/location")]
    public void OverrideCommand_Values_ShouldBeExpected(string text, string expected)
    {
        var actual = _target.OverrideCommand(text);
        
        Assert.Equal(expected, actual);
    }
}