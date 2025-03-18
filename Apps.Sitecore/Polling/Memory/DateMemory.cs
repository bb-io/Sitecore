namespace Apps.Sitecore.Polling.Memory;

public class DateMemory
{
    public DateTime LastInteractionDate { get; set; }

    public List<string> KnownItemIds { get; set; } = new List<string>();
}