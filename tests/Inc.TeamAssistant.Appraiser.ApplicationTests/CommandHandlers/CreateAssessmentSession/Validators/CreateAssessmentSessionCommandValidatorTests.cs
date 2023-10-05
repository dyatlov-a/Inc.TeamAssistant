using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.CreateAssessmentSession.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Commands.CreateAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.CreateAssessmentSession.Validators;

public sealed class CreateAssessmentSessionCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Dictionary<string, Dictionary<string, string>> _languages = new()
    {
        ["ru"] = new(),
        ["en"] = new()
    };

    private readonly Fixture _fixture = new();
    private readonly IMessageProvider _messageProvider;
    private readonly CreateAssessmentSessionCommand _validCommand;
    private readonly CreateAssessmentSessionCommandValidator _target;

    public CreateAssessmentSessionCommandValidatorTests()
    {
        _messageProvider = Substitute.For<IMessageProvider>();
        _messageProvider.Get().Returns(ServiceResult.Success(_languages));
        _validCommand = _fixture.Create<CreateAssessmentSessionCommand>() with { LanguageId = new("en") };
        _target = new(new LanguageValidator(_messageProvider), new ModeratorValidator());
    }

    [Fact]
    public void Constructor_LanguageValidatorIsNull_ThrowsException()
    {
        CreateAssessmentSessionCommandValidator Actual() => new(null!, new ModeratorValidator());

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Constructor_ModeratorValidatorIsNull_ThrowsException()
    {
        CreateAssessmentSessionCommandValidator Actual() => new(new LanguageValidator(_messageProvider), null!);

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

    [Fact]
    public async Task Validate_SetModeratorValidator_ShouldBeCalled()
    {
        var command = _validCommand with
        {
            ModeratorName = string.Empty
        };

        var actual = await _target.ValidateAsync(command);

        Assert.False(actual.IsValid);
    }
}