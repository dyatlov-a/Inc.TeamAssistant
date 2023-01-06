using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.Common.Converters;
using Inc.TeamAssistant.Appraiser.Domain;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.Common.Converters;

public sealed class AssessmentSessionConverterTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ConvertTo_StoryIsNull_ThrowsException()
    {
        AssessmentSessionDetails Story() => AssessmentSessionConverter.ConvertTo(null!);

        Assert.Throws<ArgumentNullException>(Story);
    }

    [Fact]
    public void ConvertTo_Story_ShouldBeEquals()
    {
        var assessmentSession = _fixture.Create<AssessmentSession>();

        var actual = AssessmentSessionConverter.ConvertTo(assessmentSession);

        Assert.Equal(assessmentSession.Id, actual.Id);
        Assert.Equal(assessmentSession.LanguageId, actual.LanguageId);
    }
}