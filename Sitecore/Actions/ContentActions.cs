using System.Net.Mime;
using System.Web;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;
using Sitecore.Api;
using Sitecore.Invocables;
using Sitecore.Models;
using Sitecore.Models.Requests.Item;
using Sitecore.Utils;

namespace Sitecore.Actions;

[ActionList]
public class ContentActions : SitecoreInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public ContentActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(
        invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Get item content as HTML", Description = "Get content of the specific item in HTML format")]
    public async Task<FileModel> GetItemContent([ActionParameter] ItemContentRequest input)
    {
        var endpoint = "/Content".WithQuery(input);
        var request = new SitecoreRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<Dictionary<string, string>>(request);
        var html = SitecoreHtmlConverter.ToHtml(response);

        var file = await _fileManagementClient.UploadAsync(new MemoryStream(html), MediaTypeNames.Text.Html,
            $"{input.ItemId}.html");
        return new()
        {
            File = file
        };
    }

    [Action("Update item content from HTML", Description = "Update content of the specific item from HTML file")]
    public async Task UpdateItemContent(
        [ActionParameter] ItemContentRequest itemContent,
        [ActionParameter] FileModel file,
        [ActionParameter] UpdateItemContentRequest input)
    {
        if (input.AddNewVersion is true)
        {
            itemContent.Version = null;
            await CreateItemContent(itemContent);
        }

        var htmlStream = await _fileManagementClient.DownloadAsync(file.File);
        var sitecoreFields = SitecoreHtmlConverter.ToSitecoreFields(await htmlStream.GetByteData());

        var endpoint = "/Content".WithQuery(itemContent);
        var request = new SitecoreRequest(endpoint, Method.Put, Creds);

        sitecoreFields.ToList().ForEach(x =>
            request.AddParameter($"fields[{x.Key}]", HttpUtility.UrlEncode(HttpUtility.UrlEncode(x.Value))));
        await Client.ExecuteWithErrorHandling(request);
    }

    [Action("Delete item content", Description = "Delete specific version of item's content")]
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