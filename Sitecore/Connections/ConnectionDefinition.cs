using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using Sitecore.Constants;

namespace Sitecore.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>()
    {
        new()
        {
            Name = "ApiToken",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionUsage = ConnectionUsage.Actions,
            ConnectionProperties = new List<ConnectionProperty>()
            {
                new(CredsNames.Url) { DisplayName = "Instance URL" },
                new(CredsNames.ApiKey) { DisplayName = "API key", Sensitive = true },
            }
        }
    };

    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
        Dictionary<string, string> values)
    {
        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            CredsNames.Url,
            values[CredsNames.Url]
        );

        yield return new AuthenticationCredentialsProvider(
            AuthenticationCredentialsRequestLocation.None,
            CredsNames.ApiKey,
            values[CredsNames.ApiKey]
        );
    }
}