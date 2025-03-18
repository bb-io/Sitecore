using Apps.Sitecore.Api;
using Apps.Sitecore.Invocables;
using Apps.Sitecore.Models.Entities;
using Apps.Sitecore.Models.Responses.Item;
using Apps.Sitecore.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;
using System.Globalization;
using System.Net;

namespace Apps.Sitecore.Polling;

[PollingEventList]
public class PollingList : SitecoreInvocable
{
    public PollingList(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [PollingEvent("On items created", "On new items created")]
    public Task<PollingEventResponse<DateMemory, ListItemsResponse>> OnItemsCreated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollingItemRequest input)
    {
        var endpoint = $"/Search?locale={input.Locale}&rootPath={input.RootPath}";

        return HandleItemsPolling(request, endpoint);
    }



    [PollingEvent("On items updated", "On any items updated")]
    public Task<PollingEventResponse<DateMemory, ListItemsResponse>> OnItemsUpdated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollingItemRequest input)
    {
        var dateOnly = request.Memory?.LastInteractionDate.Date.ToString("yyyy-MM-dd");
        var encodedDate = dateOnly != null ? Uri.EscapeUriString(dateOnly) : string.Empty;
        var query = $"locale={input.Locale}&rootPath={input.RootPath}&updatedAt={encodedDate}&updatedOperation=GreaterOrEqual";


        return HandleItemsPolling(request, query);
    }


    public async Task<PollingEventResponse<DateMemory, ListItemsResponse>> HandleItemsPolling(
        PollingEventRequest<DateMemory> request, string endpoint)
    {
        var items = (await Client.Paginate<ItemEntity>(
       new SitecoreRequest(endpoint, Method.Get, Creds)
   )).ToArray();

        if (items.Length == 0)
        {
            return new PollingEventResponse<DateMemory, ListItemsResponse>
            {
                FlyBird = false,
                Memory = request.Memory ?? new DateMemory { LastInteractionDate = DateTime.UtcNow }
            };
        }

        if (request.Memory == null)
        {
            var maxCreatedAt = items.Max(i => i.CreatedAt);
            var memory = new DateMemory { LastInteractionDate = maxCreatedAt };
            return new PollingEventResponse<DateMemory, ListItemsResponse>
            {
                FlyBird = false,
                Memory = memory
            };
        }

        var newItems = items.Where(i => i.CreatedAt > request.Memory.LastInteractionDate).ToArray();

        if (newItems.Any())
        {
            var maxCreatedAt = newItems.Max(i => i.CreatedAt);
            request.Memory.LastInteractionDate = maxCreatedAt;

            return new PollingEventResponse<DateMemory, ListItemsResponse>
            {
                FlyBird = true,
                Memory = request.Memory,
                Result = new ListItemsResponse(newItems)
            };
        }
        else
        {
            return new PollingEventResponse<DateMemory, ListItemsResponse>
            {
                FlyBird = false,
                Memory = request.Memory
            };
        }
    }
}