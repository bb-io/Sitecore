using Apps.Sitecore.Api;
using Apps.Sitecore.Invocables;
using Apps.Sitecore.Models.Entities;
using Apps.Sitecore.Models.Responses.Item;
using Apps.Sitecore.Polling.Memory;
using Apps.Sitecore.Polling.Requests;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Polling;
using RestSharp;

namespace Apps.Sitecore.Polling;

[PollingEventList]
public class PollingList(InvocationContext invocationContext) : SitecoreInvocable(invocationContext)
{
    [PollingEvent("On items created", "Polls for items that have been created since the last interaction date")]
    public Task<PollingEventResponse<DateMemory, PollingListItemsResponse>> OnItemsCreated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollingItemRequest input)
    {
        var endpoint = $"/Search?locale={input.Locale}&rootPath={input.RootPath}";
        return HandleItemsCreatedPolling(request, endpoint);
    }

    [PollingEvent("On items updated", "Polls for items that have been updated since the last interaction date")]
    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdatedMultiple)]
    public Task<PollingEventResponse<DateMemory, PollingListItemsResponse>> OnItemsUpdated(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollingItemRequest input)
    {
        var endpoint = $"/Search?locale={input.Locale}&rootPath={input.RootPath}";
        return HandleItemsPolling(request, endpoint, true);
    }

    [PollingEvent("On items assigned to workflow state", "Polls for items that currently have a specific workflow state")]
    public Task<PollingEventResponse<DateMemory, PollingListItemsResponse>> OnItemsWithWorkflowState(
        PollingEventRequest<DateMemory> request,
        [PollingEventParameter] PollingItemRequest input,
        [PollingEventParameter] WorkflowStateRequest workflowStateRequest)
    {
        var endpoint = $"/Search?locale={input.Locale}&rootPath={input.RootPath}&currentStateId={workflowStateRequest.WorkflowStateId}";
        return HandleItemsPolling(request, endpoint, false);
    }

    public async Task<PollingEventResponse<DateMemory, PollingListItemsResponse>> HandleItemsCreatedPolling(
        PollingEventRequest<DateMemory> request, string endpoint)
    {
        var apiRequest = new SitecoreRequest(endpoint, Method.Get, Creds);
        var items = (await Client.Paginate<PollingItemEntity>(apiRequest)).ToArray();

        if (items.Length == 0)
        {
            return new PollingEventResponse<DateMemory, PollingListItemsResponse>
            {
                FlyBird = false,
                Memory = request.Memory ?? new DateMemory { LastInteractionDate = DateTime.UtcNow }
            };
        }

        if (request.Memory == null)
        {
            var maxCreatedAt = items.Max(i => i.CreatedAt);
            var memory = new DateMemory { LastInteractionDate = maxCreatedAt };
            return new PollingEventResponse<DateMemory, PollingListItemsResponse>
            {
                FlyBird = false,
                Memory = memory
            };
        }

        var newItems = items
            .Where(i => i.CreatedAt > request.Memory.LastInteractionDate)
            .ToList();

        if (newItems.Any())
        {
            var maxCreatedAt = newItems.Max(i => i.CreatedAt);
            request.Memory.LastInteractionDate = maxCreatedAt;

            return new PollingEventResponse<DateMemory, PollingListItemsResponse>
            {
                FlyBird = true,
                Memory = request.Memory,
                Result = new(newItems)
            };
        }
        else
        {
            return new PollingEventResponse<DateMemory, PollingListItemsResponse>
            {
                FlyBird = false,
                Memory = request.Memory
            };
        }
    }

    public async Task<PollingEventResponse<DateMemory, PollingListItemsResponse>> HandleItemsPolling(
        PollingEventRequest<DateMemory> request, 
        string endpoint, 
        bool filterForUpdatedDate)
    {
        var apiRequest = new SitecoreRequest(endpoint, Method.Get, Creds);
        var items = await Client.Paginate<PollingItemEntity>(apiRequest);

        if (!items.Any())
        {
            return new PollingEventResponse<DateMemory, PollingListItemsResponse>
            {
                FlyBird = false,
                Memory = request.Memory ?? new DateMemory { LastInteractionDate = DateTime.UtcNow }
            };
        }

        if (request.Memory == null)
        {
            var maxUpdatedAt = items.Max(i => i.UpdatedAt);
            var memory = new DateMemory { LastInteractionDate = maxUpdatedAt };
            return new PollingEventResponse<DateMemory, PollingListItemsResponse>
            {
                FlyBird = false,
                Memory = memory
            };
        }

        var newItems = filterForUpdatedDate
            ? items.Where(i => i.UpdatedAt > request.Memory.LastInteractionDate)
            : items;

        if (newItems.Any())
        {
            var maxUpdatedAt = newItems.Max(i => i.UpdatedAt);
            request.Memory.LastInteractionDate = maxUpdatedAt;

            return new PollingEventResponse<DateMemory, PollingListItemsResponse>
            {
                FlyBird = true,
                Memory = request.Memory,
                Result = new(newItems.ToList())
            };
        }
        else
        {
            return new PollingEventResponse<DateMemory, PollingListItemsResponse>
            {
                FlyBird = false,
                Memory = request.Memory
            };
        }
    }
}