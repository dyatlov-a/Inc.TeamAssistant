using FluentValidation;
using Inc.TeamAssistant.Connector.Model.Queries.GetPersonPhoto;

namespace Inc.TeamAssistant.Connector.Application.QueryHandlers.GetPersonPhoto.Validators;

internal sealed class GetPersonPhotoQueryValidator : AbstractValidator<GetPersonPhotoQuery>
{
    public GetPersonPhotoQueryValidator()
    {
        RuleFor(e => e.PersonId)
            .NotEmpty();
    }
}