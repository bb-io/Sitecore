using System.Text;
using Apps.Sitecore.Models;
using Apps.Sitecore.Utils;
using HtmlAgilityPack;

namespace Tests.Sitecore;

[TestClass]
public class HtmlConverterTests
{
    private const string ItemId = "{CE1A6ABF-24CF-4BF7-9D11-4178FCE41059}";
    private const string FieldId1 = "{A60ACD61-A6DB-4182-8329-C957982CEC74}";
    private const string FieldId2 = "{75577384-3C97-45DA-A847-81B00500E250}";
    private const string SitecoreUrl = "https://sitecore.example.com/";
    private const string Locale = "en";

    private static HtmlDocument ParseHtml(byte[] bytes)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(Encoding.UTF8.GetString(bytes));
        return doc;
    }

    private static IEnumerable<FieldModel> SampleFields() =>
    [
        new() { ID = FieldId1, Value = "Hello world", Type = "Rich Text", Name = "Text" },
        new() { ID = FieldId2, Value = "My Title",    Type = "Single-Line Text", Name = "Title" }
    ];

    [TestMethod]
    public void ToHtml_SetsLangAttribute()
    {
        var doc = ParseHtml(SitecoreHtmlConverter.ToHtml(SampleFields(), ItemId, Locale, null, SitecoreUrl));
        var lang = doc.DocumentNode.SelectSingleNode("/html")?.GetAttributeValue("lang", "");
        Assert.AreEqual(Locale, lang);
    }

    [TestMethod]
    public void ToHtml_OmitsLangAttributeWhenLocaleNull()
    {
        var doc = ParseHtml(SitecoreHtmlConverter.ToHtml(SampleFields(), ItemId, null, null, SitecoreUrl));
        var htmlNode = doc.DocumentNode.SelectSingleNode("/html");
        Assert.IsNull(htmlNode?.Attributes["lang"]);
    }

    [TestMethod]
    public void ToHtml_IncludesUcid()
    {
        var doc = ParseHtml(SitecoreHtmlConverter.ToHtml(SampleFields(), ItemId, Locale, null, SitecoreUrl));
        var content = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-ucid']")?.GetAttributeValue("content", "");
        Assert.AreEqual(ItemId, content);
    }

    [TestMethod]
    public void ToHtml_IncludesContentNameWhenProvided()
    {
        var doc = ParseHtml(SitecoreHtmlConverter.ToHtml(SampleFields(), ItemId, Locale, "My Title", SitecoreUrl));
        var content = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-content-name']")?.GetAttributeValue("content", "");
        Assert.AreEqual("My Title", content);
    }

    [TestMethod]
    public void ToHtml_OmitsContentNameWhenNull()
    {
        var doc = ParseHtml(SitecoreHtmlConverter.ToHtml(SampleFields(), ItemId, Locale, null, SitecoreUrl));
        var node = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-content-name']");
        Assert.IsNull(node);
    }

    [TestMethod]
    public void ToHtml_IncludesAdminUrl()
    {
        var doc = ParseHtml(SitecoreHtmlConverter.ToHtml(SampleFields(), ItemId, Locale, null, SitecoreUrl));
        var content = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-admin-url']")?.GetAttributeValue("content", "");
        StringAssert.Contains(content, "Content Editor.aspx");
        StringAssert.Contains(content, Uri.EscapeDataString(ItemId));
        StringAssert.Contains(content, Locale);
    }

    [TestMethod]
    public void ToHtml_IncludesSystemNameAndRef()
    {
        var doc = ParseHtml(SitecoreHtmlConverter.ToHtml(SampleFields(), ItemId, Locale, null, SitecoreUrl));
        var name = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-system-name']")?.GetAttributeValue("content", "");
        var sref = doc.DocumentNode.SelectSingleNode("//meta[@name='blackbird-system-ref']")?.GetAttributeValue("content", "");
        Assert.AreEqual("Sitecore XP", name);
        StringAssert.Contains(sref, "sitecore.example.com");
    }

    [TestMethod]
    public void ToHtml_IncludesItsRevAttributesOnBody()
    {
        var doc = ParseHtml(SitecoreHtmlConverter.ToHtml(SampleFields(), ItemId, Locale, null, SitecoreUrl));
        var body = doc.DocumentNode.SelectSingleNode("//body");
        Assert.AreEqual("Sitecore XP", body?.GetAttributeValue("its-rev-tool", ""));
        StringAssert.Contains(body?.GetAttributeValue("its-rev-tool-ref", ""), "sitecore.example.com");
    }

    [TestMethod]
    public void ToHtml_AddsBlackbirdKeyToEachField()
    {
        var doc = ParseHtml(SitecoreHtmlConverter.ToHtml(SampleFields(), ItemId, Locale, null, SitecoreUrl));
        var div1 = doc.DocumentNode.SelectSingleNode($"//div[@id='{FieldId1}']");
        var div2 = doc.DocumentNode.SelectSingleNode($"//div[@id='{FieldId2}']");

        Assert.AreEqual($"{ItemId}-{FieldId1}", div1?.GetAttributeValue("data-blackbird-key", ""));
        Assert.AreEqual($"{ItemId}-{FieldId2}", div2?.GetAttributeValue("data-blackbird-key", ""));
    }

    [TestMethod]
    public void ToHtml_TitleFieldValueUsedAsContentName()
    {
        var fields = SampleFields().ToList();
        var contentName = fields
            .FirstOrDefault(f => string.Equals(f.Name, "Title", StringComparison.OrdinalIgnoreCase))
            ?.Value;

        Assert.AreEqual("My Title", contentName);
    }
}
