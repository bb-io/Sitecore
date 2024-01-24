using Sitecore.Models.Entities;

namespace Sitecore.Models.Responses.Item;

public record ListItemsResponse(IEnumerable<ItemEntity> Items);