using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ConnectToAssessmentSession.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Commands.ConnectToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.ConnectToAssessmentSession.Validators;

public sealed class ConnectToAssessmentSessionCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Dictionary<string, Dictionary<string, string>> _languages = new()
    {
        ["ru"] = new(),
        ["en"] = new()
    };

    private readonly Fixture _fixture = new();
    private readonly IMessageProvider _messageProvider;
    private readonly IMessageBuilder _messageBuilder;
    private readonly ICommandProvider _commandProvider;
    private readonly ConnectToAssessmentSessionCommand _validToAssessmentSessionCommand;
    private readonly ConnectToAssessmentSessionCommandValidator _target;

    public ConnectToAssessmentSessionCommandValidatorTests()
    {
        _messageProvider = Substitute.For<IMessageProvider>();
        _messageProvider.Get().Returns(ServiceResult.Success(_languages));

        _messageBuilder = Substitute.For<IMessageBuilder>();
        _messageBuilder.Build(Arg.Any<MessageId>(), Arg.Any<LanguageId>(), Arg.Any<object[]>()).Returns(_fixture.Create<string>());

        _commandProvider = Substitute.For<ICommandProvider>();

        _validToAssessmentSessionCommand = _fixture.Create<ConnectToAssessmentSessionCommand>() with
        {
            LanguageId = new("en")
        };
        _target = new(
            new AppraiserValidator(),
            new LanguageValidator(_messageProvider),
            _messageBuilder,
            _commandProvider);
    }

    [Fact]
    public void Constructor_AppraiserValidatorIsNull_ThrowsException()
    {
        ConnectToAssessmentSessionCommandValidator Actual() => new(
            null!,
            new LanguageValidator(_messageProvider),
            _messageBuilder,
            _commandProvider);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Constructor_LanguageValidatorIsNull_ThrowsException()
    {
        ConnectToAssessmentSessionCommandValidator Actual() => new(
            new AppraiserValidator(),
            null!,
            _messageBuilder,
            _commandProvider);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Constructor_MessageBuilderIsNull_ThrowsException()
    {
        ConnectToAssessmentSessionCommandValidator Actual() => new(
            new AppraiserValidator(),
            new LanguageValidator(_messageProvider),
            null!,
            _commandProvider);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Constructor_CommandProviderIsNull_ThrowsException()
    {
        ConnectToAssessmentSessionCommandValidator Actual() => new(
            new AppraiserValidator(),
            new LanguageValidator(_messageProvider),
            _messageBuilder,
            null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public async Task Validate_SetLanguageValidator_ShouldBeCalled()
    {
        var command = _validToAssessmentSessionCommand with {LanguageId = _fixture.Create<LanguageId>()};

        var actual = await _target.ValidateAsync(command);

        Assert.False(actual.IsValid);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldBeValid()
    {
        var actual = await _target.ValidateAsync(_validToAssessmentSessionCommand);

        Assert.True(actual.IsValid);
    }

    [Fact]
    public async Task Validate_SetAppraiserValidator_ShouldBeCalled()
    {
        var command = _validToAssessmentSessionCommand with
        {
            AppraiserName = string.Empty
        };

        var actual = await _target.ValidateAsync(command);

        Assert.False(actual.IsValid);
    }

    [Fact]
    public async Task Validate_AssessmentSessionIdIsNull_ShouldBeNotValid()
    {
        var command = _validToAssessmentSessionCommand with
        {
            AssessmentSessionId = null
        };

        var actual = await _target.ValidateAsync(command);

        Assert.False(actual.IsValid);
    }
}