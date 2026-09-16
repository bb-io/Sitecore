namespace Apps.Sitecore.Utils;

public record ItemTreeNode(string Id, string Name, string Path, DateTime UpdatedAt, bool HasChildren);
