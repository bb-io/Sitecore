using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apps.Sitecore.Actions;
using Apps.Sitecore.Models.Requests.Item;
using Tests.Sitecore.Base;

namespace Tests.Sitecore
{
    [TestClass]
    public class ItemsTests: TestBase
    {
        [TestMethod]
        public async Task SearchItems_ReturnSuccess()
        {
            var action = new ItemsActions(InvocationContext);
            var input = new SearchItemsRequest{ Locale ="es"};
            var response = await action.SearchItems(input);

            foreach (var item in response.Items)
            {
                Console.WriteLine($"{item.Name} - {item.Language}");
                Assert.IsNotNull(item);
            }
            
        }
    }
}
