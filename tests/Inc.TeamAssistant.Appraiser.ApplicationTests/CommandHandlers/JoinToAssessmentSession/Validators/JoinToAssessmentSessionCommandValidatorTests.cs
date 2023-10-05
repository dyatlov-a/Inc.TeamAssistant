using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.JoinToAssessmentSession.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Commands.JoinToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.JoinToAssessmentSession.Validators;

public sealed class JoinToAssessmentSessionCommandValidatorTests
{
    private readonly Dictionary<string, Dictionary<string, string>> _languages = new()
    {
        ["ru"] = new(),
        ["en"] = new()
    };

    private readonly Fixture _fixture = new();
    private readonly JoinToAssessmentSessionCommand _validCommand;
    private readonly JoinToAssessmentSessionCommandValidator _target;

    public JoinToAssessmentSessionCommandValidatorTests()
    {
        var messageProvider = Substitute.For<IMessageProvider>();
        messageProvider.Get().Returns(ServiceResult.Success(_languages));
        _validCommand = _fixture.Create<JoinToAssessmentSessionCommand>() with { LanguageId = new("en") };
        _target = new(new LanguageValidator(messageProvider));
    }

    [Fact]
    public void Constructor_LanguageValidatorIsNull_ThrowsException()
    {
        JoinToAssessmentSessionCommandValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldBeValid()
    {
        var actual = await _target.ValidateAsync(_validCommand);

        Assert.True(actual.IsValid);
    }

    [Fact]
    public async Task Validate_SetLanguageValidator_ShouldBeCalled()
    {
        var command = _validCommand with {LanguageId = _fixture.Create<LanguageId>()};

        var actual = await _target.ValidateAsync(command);

        Assert.False(actual.IsValid);
    }
}