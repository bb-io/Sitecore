using System.Text;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using HtmlAgilityPack;

namespace Sitecore.Utils;

public static class SitecoreHtmlConverter
{
    private const string IdAttr = "id";

    public static byte[] ToHtml(Dictionary<string, string> fields)
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlDoc.DocumentNode.AppendChild(htmlNode);
        htmlNode.AppendChild(htmlDoc.CreateElement("head"));

        var bodyNode = htmlDoc.CreateElement("body");
        htmlNode.AppendChild(bodyNode);

        fields.ToList().ForEach(x =>
        {
            var fieldNode = htmlDoc.CreateElement("div");

            fieldNode.SetAttributeValue(IdAttr, x.Key);
            fieldNode.InnerHtml = x.Value;

            bodyNode.AppendChild(fieldNode);
        });

        return Encoding.UTF8.GetBytes(htmlDoc.DocumentNode.OuterHtml);
    }

    public static Dictionary<string, string> ToSitecoreFields(byte[] html)
    {
        var htmlDoc = Encoding.UTF8.GetString(html).AsHtmlDocument();
        var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body");
        
        return bodyNode.ChildNodes.ToDictionary(x => x.Attributes[IdAttr].Value, x => x.InnerHtml);
    }
}