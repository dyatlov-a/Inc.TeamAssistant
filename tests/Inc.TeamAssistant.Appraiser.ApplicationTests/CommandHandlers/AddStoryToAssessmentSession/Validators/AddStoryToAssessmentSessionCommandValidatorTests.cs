using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStoryToAssessmentSession.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStoryToAssessmentSession;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.AddStoryToAssessmentSession.Validators;

public sealed class AddStoryToAssessmentSessionCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Dictionary<string, Dictionary<string, string>> _languages = new()
    {
        ["ru"] = new(),
        ["en"] = new()
    };

    private readonly AddStoryToAssessmentSessionOptions _options;
    private readonly Fixture _fixture = new();
    private readonly IMessageProvider _messageProvider;
    private readonly IMessageBuilder _messageBuilder;
    private readonly AddStoryToAssessmentSessionCommand _validCommand;
    private readonly AddStoryToAssessmentSessionCommandValidator _target;

    public AddStoryToAssessmentSessionCommandValidatorTests()
    {
        _options = new()
        {
            LinksPrefix = new [] { "http://", "https://" }
        };

        _messageProvider = Substitute.For<IMessageProvider>();
        _messageProvider.Get().Returns(ServiceResult.Success(_languages));

        _messageBuilder = Substitute.For<IMessageBuilder>();
        _messageBuilder.Build(Arg.Any<MessageId>(), Arg.Any<LanguageId>(), Arg.Any<object[]>()).Returns(_fixture.Create<string>());

        _validCommand = _fixture.Create<AddStoryToAssessmentSessionCommand>() with
        {
            LanguageId = new("en"),
            Links = Array.Empty<string>()
        };

        _target = new(
            _options,
            new ModeratorValidator(),
            _messageBuilder,
            Substitute.For<IAssessmentSessionRepository>(),
            new LanguageValidator(_messageProvider));
    }

    [Fact]
    public void Constructor_OptionsIsNull_ThrowsException()
    {
        AddStoryToAssessmentSessionCommandValidator Actual() => new(
            null!,
            new ModeratorValidator(),
            _messageBuilder,
            Substitute.For<IAssessmentSessionRepository>(),
            new LanguageValidator(_messageProvider));

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Constructor_ModeratorValidatorIsNull_ThrowsException()
    {
        AddStoryToAssessmentSessionCommandValidator Actual() => new(
            _options,
            null!,
            _messageBuilder,
            Substitute.For<IAssessmentSessionRepository>(),
            new LanguageValidator(_messageProvider));

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Constructor_MessageBuilderIsNull_ThrowsException()
    {
        AddStoryToAssessmentSessionCommandValidator Actual() => new(
            _options,
            new ModeratorValidator(),
            null!,
            Substitute.For<IAssessmentSessionRepository>(),
            new LanguageValidator(_messageProvider));

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Constructor_AssessmentSessionRepositoryIsNull_ThrowsException()
    {
        AddStoryToAssessmentSessionCommandValidator Actual() => new(
            _options,
            new ModeratorValidator(),
            _messageBuilder,
            null!,
            new LanguageValidator(_messageProvider));

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Constructor_LanguageValidatorIsNull_ThrowsException()
    {
        AddStoryToAssessmentSessionCommandValidator Actual() => new(
            _options,
            new ModeratorValidator(),
            _messageBuilder,
            Substitute.For<IAssessmentSessionRepository>(),
            null!);

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

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("Title", true)]
    public async Task Validate_Title_ShouldBe(string title, bool isValid)
    {
        var command = _validCommand with
        {
            Title = title
        };

        var actual = await _target.ValidateAsync(command);

        Assert.Equal(isValid, actual.IsValid);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("Link", false)]
    [InlineData("http://", false)]
    [InlineData("https://", false)]
    [InlineData("http://t.me", true)]
    [InlineData("https://t.me", true)]
    [InlineData("test1://t.me", false)]
    public async Task Validate_Links_ShouldBe(string link, bool isValid)
    {
        var command = _validCommand with
        {
            Links = new []{ link }
        };

        var actual = await _target.ValidateAsync(command);

        Assert.Equal(isValid, actual.IsValid);
    }
}