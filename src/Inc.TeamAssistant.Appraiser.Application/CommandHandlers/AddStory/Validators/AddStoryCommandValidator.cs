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
            .CustomAsync(CheckLinks);
        
        RuleFor(e => e.Teammates)
            .NotEmpty();
    }

    private async Task CheckLinks(
        IReadOnlyCollection<string> links,
        ValidationContext<AddStoryCommand> context,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(links);
        ArgumentNullException.ThrowIfNull(context);

        if (links.Count > 1)
        {
            var errorMessage = await _messageBuilder.Build(
                Messages.Appraiser_MultipleLinkError,
                context.InstanceToValidate.MessageContext.LanguageId);
            
            context.AddFailure(nameof(AddStoryCommand.Links), errorMessage);
        }
        else
        {
            var link = links.FirstOrDefault();
            
            if (!string.IsNullOrWhiteSpace(link) && link.Length > 2000)
            { 
                var errorMessage = await _messageBuilder.Build(
                    Messages.Appraiser_LinkLengthError,
                    context.InstanceToValidate.MessageContext.LanguageId);
                
                context.AddFailure(nameof(AddStoryCommand.Links), errorMessage);
            }
        }
    }
}