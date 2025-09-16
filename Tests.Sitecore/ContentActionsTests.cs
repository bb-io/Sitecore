using Apps.Sitecore.Actions;
using Apps.Sitecore.Models.Requests.Item;
using Tests.Sitecore.Base;

namespace Tests.Sitecore;

[TestClass]
public class ContentActionsTests: TestBase
{
    [TestMethod]
    public async Task SearchItems_ReturnSuccess()
    {
        var action = new ContentActions(InvocationContext, FileManager);
        var input = new SearchItemsRequest{ Locale = "en" };
        var response = await action.SearchItems(input);

        foreach (var item in response.Items)
        {
            Console.WriteLine($"{item.Id}-{item.Name} - {item.Language} - {item.Version}");
            Assert.IsNotNull(item);
        }
    }
    
    [TestMethod]
    public async Task UpdateItemContent_ReturnSuccess()
    {
        var action = new ContentActions(InvocationContext, FileManager);
        var input = new UploadContentRequest
        {
            Content = new()
            {
                Name = "{FADE8256-5F9E-4403-8D95-463CCD078DCE}.html.xlf"
            },
            Locale = "de-DE"
        };
        
        await action.UpdateItemContent(input, new());
    }
}