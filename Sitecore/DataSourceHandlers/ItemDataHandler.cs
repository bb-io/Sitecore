using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using Sitecore.Api;
using Sitecore.Invocables;
using Sitecore.Models.Entities;

namespace Sitecore.DataSourceHandlers;

public class ItemDataHandler : SitecoreInvocable, IAsyncDataSourceHandler
{
    public ItemDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new SitecoreRequest("/Search", Method.Get, Creds);
        var response = await Client.Paginate<ItemEntity>(request);

        return response
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .DistinctBy(x => x.Id)
            .Take(30)
            .ToDictionary(x => x.Id, x => x.Name);
    }
}