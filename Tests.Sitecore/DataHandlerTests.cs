using Apps.Sitecore.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Sitecore.Base;

namespace Tests.Sitecore
{
    [TestClass]
    public class DataHandlerTests : TestBase
    {
        [TestMethod]
        public async Task ItemsHandler_IsSucces()
        {
            var handler = new ItemDataHandler(InvocationContext);
            var context = new DataSourceContext
            {
                SearchString = ""
            };
            var result = await handler.GetDataAsync(context, CancellationToken.None);
            foreach (var item in result)
            {
                Console.WriteLine($"ID: {item.DisplayName}, Name: {item.Value}");
            }
            Assert.IsNotNull(result);
        }
    }
}
