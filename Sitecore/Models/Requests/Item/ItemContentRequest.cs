using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Newtonsoft.Json;
using Sitecore.DataSourceHandlers;

namespace Sitecore.Models.Requests.Item;

public class ItemContentRequest
{
    [Display("Item ID")]
    [JsonProperty("itemId")]
    [DataSource(typeof(ItemDataHandler))]
    public string ItemId { get; set; }
    
    //TODO: Add dynamic inputs
    [JsonProperty("locale")]
    public string? Locale { get; set; }
    
    [JsonProperty("version")]
    public string? Version { get; set; }
}