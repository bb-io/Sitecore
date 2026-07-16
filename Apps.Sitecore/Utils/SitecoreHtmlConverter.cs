using System.Text;
using Apps.Sitecore.Models;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using HtmlAgilityPack;

namespace Apps.Sitecore.Utils;

public static class SitecoreHtmlConverter
{
    private const string IdAttr = "id";

    public static byte[] ToHtml(IEnumerable<FieldModel> fields, string itemId, string? locale, string? contentName, string sitecoreUrl)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlDoc.DocumentNode.AppendChild(htmlNode);

        if (!string.IsNullOrEmpty(locale))
            htmlNode.SetAttributeValue("lang", locale);

        var headNode = htmlDoc.CreateElement("head");
        htmlNode.AppendChild(headNode);

        var itemIdMeta = htmlDoc.CreateElement("meta");
        itemIdMeta.SetAttributeValue("name", "blackbird-item-id");
        itemIdMeta.SetAttributeValue("content", itemId);
        headNode.AppendChild(itemIdMeta);

        AddBlackbirdMeta(htmlDoc, headNode, "ucid", itemId);
        if (!string.IsNullOrEmpty(contentName))
            AddBlackbirdMeta(htmlDoc, headNode, "content-name", contentName);
        AddBlackbirdMeta(htmlDoc, headNode, "admin-url", BuildAdminUrl(sitecoreUrl, itemId, locale));
        AddBlackbirdMeta(htmlDoc, headNode, "system-name", "Sitecore XP");
        AddBlackbirdMeta(htmlDoc, headNode, "system-ref", sitecoreUrl.TrimEnd('/'));

        var bodyNode = htmlDoc.CreateElement("body");
        htmlNode.AppendChild(bodyNode);

        bodyNode.SetAttributeValue("its-rev-tool", "Sitecore XP");
        bodyNode.SetAttributeValue("its-rev-tool-ref", sitecoreUrl.TrimEnd('/'));

        fields.ToList().ForEach(x =>
        {
            var fieldNode = htmlDoc.CreateElement("div");
            fieldNode.SetAttributeValue(IdAttr, x.ID);
            fieldNode.SetAttributeValue("data-blackbird-key", $"{itemId}-{x.ID}");
            if (!string.IsNullOrEmpty(x.Type))
                fieldNode.SetAttributeValue("data-fieldType", x.Type);
            fieldNode.InnerHtml = x.Value;
            bodyNode.AppendChild(fieldNode);
        });

        return Encoding.UTF8.GetBytes(htmlDoc.DocumentNode.OuterHtml);
    }

    public static Dictionary<string, string> ToSitecoreFields(string html)
    {
        var htmlDoc = html.AsHtmlDocument();
        var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body");

        return bodyNode.ChildNodes.ToDictionary(x => x.Attributes[IdAttr].Value, x => x.InnerHtml);
    }

    public static string? ExtractItemIdFromHtml(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);
        return doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-item-id']")?.GetAttributeValue("content", null);
    }

    public static string BuildAdminUrl(string sitecoreUrl, string itemId, string? locale)
    {
        var url = $"{sitecoreUrl.TrimEnd('/')}/sitecore/shell/Applications/Content Editor.aspx?id={Uri.EscapeDataString(itemId)}";
        if (!string.IsNullOrEmpty(locale))
            url += $"&language={Uri.EscapeDataString(locale)}";
        return url;
    }

    private static void AddBlackbirdMeta(HtmlDocument htmlDoc, HtmlNode headNode, string name, string value)
    {
        var meta = htmlDoc.CreateElement("meta");
        meta.SetAttributeValue("name", $"blackbird-{name}");
        meta.SetAttributeValue("content", value);
        headNode.AppendChild(meta);
    }
}
