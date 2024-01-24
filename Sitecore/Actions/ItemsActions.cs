using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;
using Sitecore.Api;
using Sitecore.Invocables;
using Sitecore.Models.Entities;
using Sitecore.Models.Requests.Item;
using Sitecore.Models.Responses.Item;

namespace Sitecore.Actions;

[ActionList]
public class ItemsActions : SitecoreInvocable
{
    public ItemsActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("Search items", Description = "Search items based on provided criterias")]
    public async Task<ListItemsResponse> SearchItems([ActionParameter] SearchItemsRequest input)
    {
        var endpoint = "/Search".WithQuery(input);
        var request = new SitecoreRequest(endpoint, Method.Get, Creds);

        var response = await Client.Paginate<ItemEntity>(request);
        return new(response);
    }
}