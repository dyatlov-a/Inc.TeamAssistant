using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.Common.Converters;

public sealed class SummaryByStoryConverterTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ConvertTo_AssessmentSessionIsNull_ThrowsException()
    {
        SummaryByStory Story() => SummaryByStoryConverter.ConvertTo(null!);

        Assert.Throws<ArgumentNullException>(Story);
    }

    [Fact]
    public void ConvertTo_AssessmentSession_ShouldBeEquals()
    {
        var story = _fixture.Create<AssessmentSession>();

        var actual = SummaryByStoryConverter.ConvertTo(story);

        Assert.Equal(story.Id, actual.AssessmentSessionId);
        Assert.Equal(story.LanguageId, actual.AssessmentSessionLanguageId);
        Assert.Equal(story.ChatId, actual.ChatId);
    }
}