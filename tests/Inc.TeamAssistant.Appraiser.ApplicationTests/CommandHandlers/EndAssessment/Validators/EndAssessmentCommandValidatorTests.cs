using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.FinishAssessmentSession.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.FinishAssessmentSession;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.EndAssessment.Validators;

public sealed class EndAssessmentCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Fixture _fixture = new();
    private readonly FinishAssessmentSessionCommand _validSessionCommand;
    private readonly FinishAssessmentSessionCommandValidator _target;

    public EndAssessmentCommandValidatorTests()
    {
        _validSessionCommand = _fixture.Create<FinishAssessmentSessionCommand>();
        _target = new(new ModeratorValidator());
    }

    [Fact]
    public void Constructor_ModeratorValidatorIsNull_ThrowsException()
    {
        FinishAssessmentSessionCommandValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Validate_ValidCommand_ShouldBeValid()
    {
        var actual = _target.Validate(_validSessionCommand);

        Assert.True(actual.IsValid);
    }

    [Fact]
    public void Validate_SetModeratorValidator_ShouldBeCalled()
    {
        var command = _validSessionCommand with
        {
            ModeratorName = string.Empty
        };

        var actual = _target.Validate(command);

        Assert.False(actual.IsValid);
    }
}