using Apps.Sitecore.Api;
using Apps.Sitecore.Invocables;
using Apps.Sitecore.Models.Entities;
using Apps.Sitecore.Models.Responses.Item;
using Apps.Sitecore.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;
using System.Globalization;

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
        var lastInteraction = request.Memory?.LastInteractionDate.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ", CultureInfo.InvariantCulture);
        var encodedDate = lastInteraction != null ? Uri.EscapeDataString(lastInteraction) : string.Empty;
        var query = $"locale={input.Locale}&rootPath={input.RootPath}&createdAt={encodedDate}&createdOperation=GreaterOrEqual";

        return HandleItemsPolling(request, query);
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
        PollingEventRequest<DateMemory> request, string query)
    {
        if (request.Memory == null)
        {
            return new()
            {
                FlyBird = false,
                Memory = new()
                {
                    LastInteractionDate = DateTime.UtcNow
                }
            };
        }

        var endpoint = $"/Search?{query}";
        var items = (await Client.Paginate<ItemEntity>(new SitecoreRequest(endpoint, Method.Get, Creds))).ToArray();

        if (items.Length == 0)
        {
            return new PollingEventResponse<DateMemory, ListItemsResponse>
            {
                FlyBird = false,
                Memory = new DateMemory
                {
                    LastInteractionDate = request.Memory.LastInteractionDate
                }
            };
        }

        var newLastInteraction = items.Max(item => item.CreatedAt.ToUniversalTime());

        return new PollingEventResponse<DateMemory, ListItemsResponse>
        {
            FlyBird = true,
            Memory = new DateMemory
            {
                LastInteractionDate = newLastInteraction
            },
            Result = new ListItemsResponse(items)
        };
    }
}