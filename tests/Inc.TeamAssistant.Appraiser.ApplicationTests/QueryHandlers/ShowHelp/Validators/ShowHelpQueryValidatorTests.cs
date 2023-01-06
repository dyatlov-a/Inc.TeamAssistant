using AutoFixture;
using Inc.TeamAssistant.Appraiser.Application.Common.Validators;
using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Application.QueryHandlers.ShowHelp.Validators;
using Inc.TeamAssistant.Appraiser.Model;
using Inc.TeamAssistant.Appraiser.Model.Common;
using Inc.TeamAssistant.Appraiser.Model.Queries.ShowHelp;
using Inc.TeamAssistant.Appraiser.Primitives;
using NSubstitute;
using Xunit;

namespace Inc.TeamAssistant.Appraiser.ApplicationTests.QueryHandlers.ShowHelp.Validators;

public sealed class ShowHelpQueryValidatorTests
{
    private readonly Dictionary<string, Dictionary<string, string>> _languages = new()
    {
        ["ru"] = new(),
        ["en"] = new()
    };

    private readonly Fixture _fixture = new();
    private readonly IMessageProvider _messageProvider;
    private readonly IMessageBuilder _messageBuilder;
    private readonly ShowHelpQuery _validQuery;
    private readonly ShowHelpQueryValidator _target;

    public ShowHelpQueryValidatorTests()
    {
        _messageProvider = Substitute.For<IMessageProvider>();
        _messageProvider.Get().Returns(ServiceResult.Success(_languages));

        _messageBuilder = Substitute.For<IMessageBuilder>();
        _messageBuilder.Build(Arg.Any<MessageId>(), Arg.Any<LanguageId>(), Arg.Any<object[]>()).Returns(_fixture.Create<string>());

        _validQuery = new(new("en"));
        _target = new ShowHelpQueryValidator(new LanguageValidator(_messageProvider));
    }

    [Fact]
    public void Constructor_OptionsIsNull_ThrowsException()
    {
        ShowHelpQueryValidator Actual() => new(null!);

        Assert.Throws<ArgumentNullException>(Actual);
    }

    [Fact]
    public async Task Validate_SetLanguageValidator_ShouldBeCalled()
    {
        var query = new ShowHelpQuery(_fixture.Create<LanguageId>());

        var actual = await _target.ValidateAsync(query);

        Assert.False(actual.IsValid);
    }

    [Fact]
    public async Task Validate_ValidQuery_ShouldBeValid()
    {
        var actual = await _target.ValidateAsync(_validQuery);

        Assert.True(actual.IsValid);
    }
}