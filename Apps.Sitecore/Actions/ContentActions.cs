using System.Net.Mime;
using System.Text;
using System.Web;
using Apps.Sitecore.Api;
using Apps.Sitecore.Invocables;
using Apps.Sitecore.Models;
using Apps.Sitecore.Models.Entities;
using Apps.Sitecore.Models.Requests.Item;
using Apps.Sitecore.Models.Responses.Item;
using Apps.Sitecore.Utils;
using Blackbird.Applications.SDK.Blueprints;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using RestSharp;

namespace Apps.Sitecore.Actions;

[ActionList("Content")]
public class ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient)
    : SitecoreInvocable(invocationContext)
{
    [Action("Search content", Description = "Search content based on provided criteria")]
    [BlueprintActionDefinition(BlueprintAction.SearchContent)]
    public async Task<ListItemsResponse> SearchItems([ActionParameter] SearchItemsRequest input)
    {
        var endpoint = "/Search".WithQuery(input);
        var request = new SitecoreRequest(endpoint, Method.Get, Creds);

        var items = await Client.Paginate<ItemEntity>(request);

        var latestItems = items
            .GroupBy(item => item.Id)
            .Select(g => g.OrderByDescending(item =>
            {
                int versionNumber;
                return int.TryParse(item.Version, out versionNumber) ? versionNumber : 0;
            }).First())
            .ToList();

        return new ListItemsResponse(latestItems);
    }
    
    [Action("Download content", Description = "Get localizable fields of the specific item")]
    [BlueprintActionDefinition(BlueprintAction.DownloadContent)]
    public async Task<FileModel> GetItemContent([ActionParameter] ItemContentRequest input)
    {
        var endpoint = "/Content".WithQuery(input);
        var request = new SitecoreRequest(endpoint, Method.Get, Creds);
        IEnumerable<FieldModel> response;
        try 
        {
            response = await Client.ExecuteWithErrorHandling<IEnumerable<FieldModel>>(request);
        }
        catch 
        {
           var oldresponse = await Client.ExecuteWithErrorHandling<Dictionary<string, string>>(request);
            response = oldresponse.Select(x => new FieldModel { ID = x.Key, Value = x.Value }).ToArray();
        }

        var html = SitecoreHtmlConverter.ToHtml(response, input.ContentId);

        var file = await fileManagementClient.UploadAsync(new MemoryStream(html), MediaTypeNames.Text.Html,
            $"{input.ContentId}.html");
        return new()
        {
            Content = file
        };
    }

    [Action("Upload content", Description = "Upload localizable fields to the specific item from a file")]
    [BlueprintActionDefinition(BlueprintAction.UploadContent)]
    public async Task UpdateItemContent([ActionParameter] UploadContentRequest uploadContentRequest,
        [ActionParameter] UpdateItemContentRequest input)
    {
        var htmlStream = await fileManagementClient.DownloadAsync(uploadContentRequest.Content);
        var bytes = await htmlStream.GetByteData();
        var html = Encoding.UTF8.GetString(bytes);
        if (Xliff2Serializer.IsXliff2(html))
        {
            html = Transformation.Parse(html, uploadContentRequest.Content.Name).Target().Serialize();
            if (html == null) throw new PluginMisconfigurationException("XLIFF did not contain any files");
        }
        
        var extractedItemId = SitecoreHtmlConverter.ExtractItemIdFromHtml(html);
        
        var itemId = uploadContentRequest.ContentId ?? extractedItemId ?? throw new Exception("Didn't find item Item ID in the HTML file. Please provide it in the input.");
        uploadContentRequest.ContentId = itemId;

        if (input.AddNewVersion is true)
        {
            uploadContentRequest.Version = null;
            await CreateItemContent(new ItemContentRequest { ContentId = uploadContentRequest.ContentId, Version = uploadContentRequest.Version, Locale = uploadContentRequest.Locale});
        }

        var sitecoreFields = SitecoreHtmlConverter.ToSitecoreFields(html);

        var request = new SitecoreRequest("/Content", Method.Put, Creds);
        if(!string.IsNullOrEmpty(uploadContentRequest.Locale))
        {
            request.AddParameter("locale", uploadContentRequest.Locale);
        }
        if(!string.IsNullOrEmpty(uploadContentRequest.ContentId))
        {
            request.AddParameter("itemId", uploadContentRequest.ContentId);
        }
        if(!string.IsNullOrEmpty(uploadContentRequest.Version))
        {
            request.AddParameter("version", uploadContentRequest.Version);
        }

        sitecoreFields.ToList().ForEach(x =>
            request.AddParameter($"fields[{x.Key}]", HttpUtility.UrlEncode(HttpUtility.UrlEncode(x.Value))));
        await Client.ExecuteWithErrorHandling(request);
    }
    
    [Action("Get IDs from item content", Description = "Get Item ID from the HTML or JSON content file")]
    public async Task<GetItemIdFromHtmlResponse> GetItemIdFromHtml([ActionParameter] FileModel file)
    {
        var htmlStream = await fileManagementClient.DownloadAsync(file.Content);
        var bytes = await htmlStream.GetByteData();
        var html = Encoding.UTF8.GetString(bytes);
        var itemId = SitecoreHtmlConverter.ExtractItemIdFromHtml(html);
        
        return new GetItemIdFromHtmlResponse
        {
            ItemId = itemId ?? string.Empty
        };
    }

    [Action("Delete content", Description = "Delete specific version of item's content")]
    public Task DeleteItemContent([ActionParameter] ItemContentRequest input)
    {
        var endpoint = "/Content".WithQuery(input);
        var request = new SitecoreRequest(endpoint, Method.Delete, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }

    private Task CreateItemContent([ActionParameter] ItemContentRequest input)
    {
        var endpoint = "/Content".WithQuery(input);
        var request = new SitecoreRequest(endpoint, Method.Post, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }
}