using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.Common.Converters;

public sealed class StoryConverterTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ConvertTo_StoryIsNull_ThrowsException()
    {
        StoryDetails Story() => StoryConverter.ConvertTo(null!);

        Assert.Throws<ArgumentNullException>(Story);
    }

    [Fact]
    public void ConvertTo_Story_ShouldBeEquals()
    {
        var story = _fixture.Create<Story>();

        var actual = StoryConverter.ConvertTo(story);

        Assert.Equal(story.Title, actual.Title);
        Assert.Equal(story.Links, actual.Links);
    }
}