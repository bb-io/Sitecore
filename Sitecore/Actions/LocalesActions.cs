using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using Sitecore.Api;
using Sitecore.Invocables;
using Sitecore.Models.Entities;
using Sitecore.Models.Responses.Locale;

namespace Sitecore.Actions;

[ActionList]
public class LocalesActions : SitecoreInvocable
{
    public LocalesActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Get all configured languages", Description = "List all available languages")]
    public async Task<ListLocalesResponse> ListLocales()
    {
        var request = new SitecoreRequest("/Locales", Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<LocaleEntity[]>(request);
        return new(response);
    }
}