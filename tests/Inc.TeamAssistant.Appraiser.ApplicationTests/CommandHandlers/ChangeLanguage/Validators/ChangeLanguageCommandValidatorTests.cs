using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.CommandHandlers.ChangeLanguage.Validators;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Commands.ChangeLanguage;
using Inc.TeamAssistant.Appraiser.Model.Common;
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
    private readonly IMessageProvider _messageProvider;
    private readonly ChangeLanguageCommand _validCommand;
    private readonly ChangeLanguageCommandValidator _target;

    public ChangeLanguageCommandValidatorTests()
    {
        _messageProvider = Substitute.For<IMessageProvider>();
        _messageProvider.Get().Returns(ServiceResult.Success(_languages));
        _validCommand = _fixture.Create<ChangeLanguageCommand>() with {LanguageId = new("en")};
        _target = new(new LanguageValidator(_messageProvider), new ModeratorValidator());
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
        ChangeLanguageCommandValidator Actual() => new(new LanguageValidator(_messageProvider), null!);

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