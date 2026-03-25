using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Newtonsoft.Json;

namespace Apps.Sitecore.Models.Entities;

public class PollingItemEntity : BaseItemEntity, IDownloadContentInput
{
    [Display("Content ID"), JsonProperty("id")]
    public string ContentId { get; set; } = string.Empty;
}
