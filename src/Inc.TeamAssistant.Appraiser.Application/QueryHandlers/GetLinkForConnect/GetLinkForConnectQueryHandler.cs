using Inc.TeamAssistant.Appraiser.Application.Contracts;
using Inc.TeamAssistant.Appraiser.Model.Queries.GetLinkForConnect;
using Inc.TeamAssistant.Primitives;
using MediatR;

namespace Inc.TeamAssistant.Appraiser.Application.QueryHandlers.GetLinkForConnect;

internal sealed class GetLinkForConnectQueryHandler : IRequestHandler<GetLinkForConnectQuery, GetLinkForConnectResult>
{
    private readonly ILinkBuilder _linkBuilder;
    private readonly IQuickResponseCodeGenerator _codeGenerator;
    private readonly AppraiserOptions _options;

    public GetLinkForConnectQueryHandler(
        ILinkBuilder linkBuilder,
        IQuickResponseCodeGenerator codeGenerator,
        AppraiserOptions options)
    {
        _linkBuilder = linkBuilder ?? throw new ArgumentNullException(nameof(linkBuilder));
        _codeGenerator = codeGenerator ?? throw new ArgumentNullException(nameof(codeGenerator));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<GetLinkForConnectResult> Handle(GetLinkForConnectQuery query, CancellationToken cancellationToken)
    {
        if (query is null)
            throw new ArgumentNullException(nameof(query));

        var link = _linkBuilder.BuildLinkMoveToBot(_options.BotName);
        var code = _codeGenerator.Generate(link);

        return Task.FromResult(new GetLinkForConnectResult(code));
    }
}