using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.Common.Validators;

public sealed class ModeratorValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly AddStoryToAssessmentSessionCommand _validToAssessmentSessionCommand;
    private readonly ModeratorValidator _target;

    public ModeratorValidatorTests()
    {
        var fixture = new Fixture();

        _validToAssessmentSessionCommand = fixture.Create<AddStoryToAssessmentSessionCommand>() with
        {
            Links = Array.Empty<string>()
        };
        _target = new();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldBeValid()
    {
        var actual = _target.Validate(_validToAssessmentSessionCommand);

        Assert.True(actual.IsValid);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("ModeratorName", true)]
    public void Validate_ModeratorName_ShouldBe(string moderatorName, bool isValid)
    {
        var command = _validToAssessmentSessionCommand with
        {
            ModeratorName = moderatorName
        };

        var actual = _target.Validate(command);

        Assert.Equal(isValid, actual.IsValid);
    }
}