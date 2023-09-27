using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;
using Sitecore.Dtos;
using Sitecore.Models.Requests;
using Sitecore.Models.Responses;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Web;
using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Sitecore
{
    [ActionList]
    public class Actions : BaseInvocable
    {
        public Actions(InvocationContext invocationContext) : base(invocationContext)
        {
        }

        [Action("Get item", Description = "Get item")]
        public ItemDto GetItem([ActionParameter] GetItemRequest input)
        {
            var client = new SitecoreClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new SitecoreRequest($"/sitecore/api/ssc/item/{input.Id}", Method.Get,
                InvocationContext.AuthenticationCredentialsProviders);
            if (input.Database != null) { request.AddQueryParameter("database", input.Database); }
            else { request.AddQueryParameter("database", "master"); }
            return client.Execute<ItemDto>(request).Data;
        }

        [Action("Get item as HTML file", Description = "Get item as HTML file")]
        public FileResponse GetItemAsHtml([ActionParameter] GetItemRequest input)
        {
            var result = GetItem(input);
            string htmlFile =
            $"<html><head><title>{result.Title}</title></head><body>{result.Text}</body></html>";
            return new FileResponse() {
                File = new File(Encoding.UTF8.GetBytes(htmlFile))
                {
                    Name = $"{result.ItemName}.html",
                    ContentType = MediaTypeNames.Text.Html
                }
            };
        }

        [Action("Create new item", Description = "Create new item")]
        public void CreateItem([ActionParameter] CreateItemRequest input)
        {
            var client = new SitecoreClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new SitecoreRequest($"/sitecore/api/ssc/item/{HttpUtility.UrlEncode(input.ItemFolder)}", Method.Post,
                InvocationContext.AuthenticationCredentialsProviders);
            if (input.Database != null) { request.AddQueryParameter("database", input.Database); }
            else { request.AddQueryParameter("database", "master"); }
            request.AddStringBody(JsonSerializer.Serialize(new
            {
                ItemName = input.ItemName,
                TemplateID = input.TemplateID,
                Title = input.Title
            }), DataFormat.Json);
            client.Execute(request);
        }

        [Action("Get item's children", Description = "Get item's children")]
        public GetItemsChildrenResponse GetItemsChildren([ActionParameter] GetItemRequest input)
        {
            var client = new SitecoreClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new SitecoreRequest($"/sitecore/api/ssc/item/{input.Id}/children", Method.Get,
                InvocationContext.AuthenticationCredentialsProviders);
            if (input.Database != null) { request.AddQueryParameter("database", input.Database); }
            else { request.AddQueryParameter("database", "master"); }
            return new GetItemsChildrenResponse() { Items = client.Execute<List<ItemDto>>(request).Data };
        }

        [Action("Update item", Description = "Update item")]
        public void UpdateItem([ActionParameter] UpdateItemRequest input)
        {
            var client = new SitecoreClient(InvocationContext.AuthenticationCredentialsProviders);
            var request = new SitecoreRequest($"/sitecore/api/ssc/item/{input.ItemID}", Method.Patch,
                InvocationContext.AuthenticationCredentialsProviders);
            if (input.Database != null) { request.AddQueryParameter("database", input.Database); }
            else { request.AddQueryParameter("database", "master"); }
            if (input.ItemLanguage != null) { request.AddQueryParameter("language", input.ItemLanguage); }
            request.AddStringBody(JsonSerializer.Serialize(new
            {
                input.ItemName,
                input.ItemLanguage,
                input.DisplayName,
                input.ItemIcon,
                input.Text,
                input.Title
            }, new JsonSerializerOptions() { DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull}), DataFormat.Json);
            client.Execute(request);
        }

    }
}