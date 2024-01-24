using Blackbird.Applications.Sdk.Common;

namespace Sitecore.Models.Entities;

public class LocaleEntity
{
    [Display("Display name")] public string DisplayName { get; set; }

    public string Language { get; set; }

    [Display("Is primary")] public bool IsPrimary { get; set; }
}