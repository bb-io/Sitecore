using Apps.Sitecore.Models.Entities;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Sitecore.Models.Responses.Item;

public record PollingListItemsResponse(List<PollingItemEntity> Items) : IMultiDownloadableContentOutput<PollingItemEntity>
{
    [Display("Items")]
    public List<PollingItemEntity> Items { get; set; } = Items;
}
