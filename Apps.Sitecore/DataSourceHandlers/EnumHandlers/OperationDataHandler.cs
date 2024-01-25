using Blackbird.Applications.Sdk.Utils.Sdk.DataSourceHandlers;

namespace Sitecore.DataSourceHandlers.EnumHandlers;

public class OperationDataHandler : EnumDataHandler
{
    protected override Dictionary<string, string> EnumValues => new()
    {
        { "GreaterOrEqual", "Greater or equal" },
        { "LessOrEqual", "Less or equal" },
        { "Equal", "Equal" },
    };
}