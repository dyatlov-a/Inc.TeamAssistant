using FluentValidation;
using Inc.TeamAssistant.Appraiser.Model.Commands.AddStory;
using Inc.TeamAssistant.Primitives.Extensions;
using Inc.TeamAssistant.Primitives.Languages;

namespace Inc.TeamAssistant.Appraiser.Application.CommandHandlers.AddStory.Validators;

internal sealed class AddStoryCommandValidator : AbstractValidator<AddStoryCommand>
{
    private readonly IMessageBuilder _messageBuilder;
    
    public AddStoryCommandValidator(IMessageBuilder messageBuilder)
    {
        _messageBuilder = messageBuilder ?? throw new ArgumentNullException(nameof(messageBuilder));
        
        RuleFor(e => e.TeamId)
            .NotEmpty();
        
        RuleFor(e => e.StoryType)
            .NotEmpty();
        
        RuleFor(e => e.Title)
            .NotEmpty()
            .Must(e => !e.HasCommand())
            .WithMessage("'{PropertyName}' please enter text value.");
        
        RuleFor(e => e.Links)
            .Custom(CheckLinks);
        
        RuleFor(e => e.Teammates)
            .NotEmpty();
    }

    private void CheckLinks(IReadOnlyCollection<string> links, ValidationContext<AddStoryCommand> context)
    {
        ArgumentNullException.ThrowIfNull(links);
        ArgumentNullException.ThrowIfNull(context);
        
        const int maxLinksLength = 2000;
        const int maxLinksCount = 1;
        const string propertyName = nameof(AddStoryCommand.Links);
        
        var languageId = context.InstanceToValidate.MessageContext.LanguageId;

        if (links.Count > maxLinksCount)
        {
            var errorMessage = _messageBuilder.Build(Messages.Appraiser_MultipleLinkError, languageId);
            
            context.AddFailure(propertyName, errorMessage);
        }
        else
        {
            var link = links.SingleOrDefault();
            
            if (!string.IsNullOrWhiteSpace(link) && link.Length > maxLinksLength)
            { 
                var errorMessage = _messageBuilder.Build(Messages.Appraiser_LinkLengthError, languageId);
                
                context.AddFailure(propertyName, errorMessage);
            }
        }
    }
}