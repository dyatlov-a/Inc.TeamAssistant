using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ChangeLanguage.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Common.Messages;
using Inc.TeamAssistant.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.CommandHandlers.ChangeLanguage.Validators;

public sealed class ChangeLanguageCommandValidatorTests : IClassFixture<ValidatorOptionsFixture>
{
    private readonly Dictionary<string, Dictionary<string, string>> _languages = new()
    {
        ["ru"] = new(),
        ["en"] = new()
    };

    private readonly Fixture _fixture = new();
    private readonly IMessageService _messageService;
    private readonly ChangeLanguageCommand _validCommand;
    private readonly ChangeLanguageCommandValidator _target;

    public ChangeLanguageCommandValidatorTests()
    {
        _messageService = Substitute.For<IMessageService>();
        _messageService.GetAll(Arg.Any<CancellationToken>()).Returns(ServiceResult.Success(_languages));
        _validCommand = _fixture.Create<ChangeLanguageCommand>() with {LanguageId = new("en")};
        _target = new(new LanguageValidator(_messageService), new ModeratorValidator());
    }

    [Fact]
    public void Constructor_LanguageValidatorIsNull_ThrowsException()
    {
        ChangeLanguageCommandValidator Actual() => new(null!, new ModeratorValidator());

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public void Constructor_ModeratorValidatorIsNull_ThrowsException()
    {
        ChangeLanguageCommandValidator Actual() => new(new LanguageValidator(_messageService), null!);

        Assert.Throws<ArgumentNullException>(Actual);
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
        var command = _validCommand with {ModeratorName = string.Empty};

        var actual = await _target.ValidateAsync(command);

        Assert.False(actual.IsValid);
    }
}