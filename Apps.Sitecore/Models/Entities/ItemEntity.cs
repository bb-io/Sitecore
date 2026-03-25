using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Sitecore.Models.Entities;

public class ItemEntity : BaseItemEntity, IContentOutput
{
    [Display("Content ID")]
    public string Id { get; set; } = string.Empty;
}