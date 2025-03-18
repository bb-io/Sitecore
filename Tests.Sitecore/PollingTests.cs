using Apps.Sitecore.Polling;
using Apps.Sitecore.Polling.Memory;
using Blackbird.Applications.Sdk.Common.Polling;
using Tests.Sitecore.Base;

namespace Tests.Sitecore
{
    [TestClass]
    public class PollingTests : TestBase
    {
        [TestMethod]
        public async Task OnItesCreated_IsSuccess()
        {
            var polling = new PollingList(InvocationContext);
            var initialMemory = new DateMemory
            {
                LastInteractionDate = new DateTime(2024, 9, 9)
            };

            var request = new PollingEventRequest<DateMemory>
            {
                Memory = initialMemory
            };

            var input = new PollingItemRequest
            {
                Locale = "en",
                RootPath = "/sitecore/content/home/dogs"
            };

            var result = await polling.OnItemsCreated(request, input);

            foreach (var item in result.Result.Items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Language: {item.Language}, FullPath: {item.FullPath}, CreatedAt: {item.CreatedAt}");
            }
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task OnItesUpdated_IsSuccess()
        {
            var polling = new PollingList(InvocationContext);
            var initialMemory = new DateMemory
            {
                LastInteractionDate = new DateTime(2015, 1, 1)
            };

            var request = new PollingEventRequest<DateMemory>
            {
                Memory = initialMemory
            };

            var input = new PollingItemRequest
            {
                Locale = "en",
                RootPath = "/sitecore/content/home/del"
            };

            var result = await polling.OnItemsUpdated(request, input);

            foreach (var item in result.Result.Items)
            {
                Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Language: {item.Language}, FullPath: {item.FullPath}, CreatedAt: {item.CreatedAt}");
            }
            Assert.IsNotNull(result);
        }
    }
}
