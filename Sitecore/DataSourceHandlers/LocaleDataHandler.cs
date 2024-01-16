using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using Sitecore.Api;
using Sitecore.Invocables;
using Sitecore.Models.Entities;

namespace Sitecore.DataSourceHandlers;

public class LocaleDataHandler : SitecoreInvocable, IAsyncDataSourceHandler
{
    public LocaleDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new SitecoreRequest("/Locales", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<LocaleEntity[]>(request);

        return response
            .Where(x => context.SearchString is null ||
                        x.DisplayName.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .ToDictionary(x => x.Language, x => x.DisplayName.Split(':').First().Trim());
    }
}