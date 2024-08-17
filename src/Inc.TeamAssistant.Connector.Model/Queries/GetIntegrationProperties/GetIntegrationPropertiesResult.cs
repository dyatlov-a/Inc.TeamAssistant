using Inc.TeamAssistant.Primitives;

namespace Inc.TeamAssistant.Connector.Model.Queries.GetIntegrationProperties;

public sealed record GetIntegrationPropertiesResult(
    IntegrationProperties? Properties,
    bool HasManagerAccess,
    IReadOnlyCollection<Person> Teammates);