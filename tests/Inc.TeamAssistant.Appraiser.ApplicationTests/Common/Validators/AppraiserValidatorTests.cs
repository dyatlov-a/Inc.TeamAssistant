using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.ExitFromAssessmentSession;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.Common.Validators;

public sealed class AppraiserValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly ExitFromAssessmentSessionCommand _validSessionCommand;
    private readonly AppraiserValidator _target;

    public AppraiserValidatorTests()
    {
        var fixture = new Fixture();

        _validSessionCommand = fixture.Create<ExitFromAssessmentSessionCommand>();
        _target = new();
    }

    [Fact]
    public void Validate_ValidCommand_ShouldBeValid()
    {
        var actual = _target.Validate(_validSessionCommand);

        Assert.True(actual.IsValid);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("AppraiserName", true)]
    public void Validate_ModeratorName_ShouldBe(string appraiserName, bool isValid)
    {
        var command = _validSessionCommand with
        {
            AppraiserName = appraiserName
        };

        var actual = _target.Validate(command);

        Assert.Equal(isValid, actual.IsValid);
    }
}