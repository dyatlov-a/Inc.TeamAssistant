using Inc.TeamAssistant.Connector.Application.Alias;
using Xunit;

namespace Inc.TeamAssistant.Connector.ApplicationTests.Alias;

public sealed class AliasFactoryTests
{
    [Fact]
    public void Create_ShouldBeExpected()
    {
        var aliases = new []
        {
            new AliasValue("/l", "/location"),
            new AliasValue("/nr", "/need_review")
        };

        var actual = AliasFactory.Create().ToArray();

        Assert.Equal(aliases.Length, actual.Length);
        
        foreach (var alias in aliases)
            Assert.Contains(alias, actual);
    }
}