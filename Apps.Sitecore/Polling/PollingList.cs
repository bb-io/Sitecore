using Apps.Sitecore.Api;
using Apps.Sitecore.Invocables;
using Apps.Sitecore.Models.Entities;
using Apps.Sitecore.Models.Responses.Item;
using Apps.Sitecore.Polling.Memory;
using Apps.Sitecore.Polling.Requests;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Sitecore.Polling;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : SitecoreInvocable(invocationContext)
{
    [PollingEvent("On items created", "On new items created")]
    public Task<PollingEventResponse<DateMemory, ListItemsResponse>> OnItemsCreated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollingItemRequest input)
    {
        var endpoint = $"/Search?locale={input.Locale}&rootPath={input.RootPath}";
        return HandleItemsCreatedPolling(request, endpoint);
    }

    [PollingEvent("On items updated", "On any items updated")]
    public Task<PollingEventResponse<DateMemory, ListItemsResponse>> OnItemsUpdated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollingItemRequest input)
    {
        var endpoint = $"/Search?locale={input.Locale}&rootPath={input.RootPath}";
        return HandleItemsPolling(request, endpoint, true);
    }

    [PollingEvent("On items workflow state reached", "Triggered when items reach a specific workflow state")]
    public Task<PollingEventResponse<DateMemory, ListItemsResponse>> OnItemsWorkflowStateChanged(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollingItemRequest input,
        [PollingEventParameter] WorkflowStateRequest workflowStateRequest)
    {
        var endpoint = $"/Search?locale={input.Locale}&rootPath={input.RootPath}&currentStateId={workflowStateRequest.WorkflowStateId}";
        return HandleItemsPolling(request, endpoint, false);
    }

    public async Task<PollingEventResponse<DateMemory, ListItemsResponse>> HandleItemsCreatedPolling(PollingEventRequest<DateMemory> request, string endpoint)
    {
        var apiRequest = new SitecoreRequest(endpoint, Method.Get, Creds);
        var items = (await Client.Paginate<ItemEntity>(apiRequest)).ToArray();

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

    public async Task<PollingEventResponse<DateMemory, ListItemsResponse>> HandleItemsPolling(PollingEventRequest<DateMemory> request, string endpoint, bool filterForUpdatedDate)
    {
        var apiRequest = new SitecoreRequest(endpoint, Method.Get, Creds);
        var itemsEnumerable = await Client.Paginate<ItemEntity>(apiRequest);
        var items = itemsEnumerable.ToArray();

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
            var maxUpdatedAt = items.Max(i => i.UpdatedAt);
            var memory = new DateMemory { LastInteractionDate = maxUpdatedAt };
            return new PollingEventResponse<DateMemory, ListItemsResponse>
            {
                FlyBird = false,
                Memory = memory
            };
        }

        var newItems = filterForUpdatedDate
            ? items.Where(i => i.UpdatedAt > request.Memory.LastInteractionDate).ToArray()
            : items;

        if (newItems.Any())
        {
            var maxUpdatedAt = newItems.Max(i => i.UpdatedAt);
            request.Memory.LastInteractionDate = maxUpdatedAt;

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