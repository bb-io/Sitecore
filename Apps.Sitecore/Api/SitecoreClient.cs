using Apps.Sitecore.Constants;
using Apps.Sitecore.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using HtmlAgilityPack;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Sitecore.Api;

public class SitecoreClient : BlackBirdRestClient
{
    private InvocationContext invocationContext;

    public SitecoreClient(IEnumerable<AuthenticationCredentialsProvider> creds, InvocationContext invocationContext) :
        base(new()
        {
            BaseUrl = creds.Get(CredsNames.Url).Value.ToUri().Append("api/blackbird")
        })
    {
        invocationContext = invocationContext ?? new InvocationContext();
    }

    public async Task<IEnumerable<T>> Paginate<T>(RestRequest request)
    {
        // The pagination on API side is bad and implemented in a in-memory way, so simply getting all the data by 1 request
        var result = await ExecuteWithErrorHandling<T[]>(request);
        return result;
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        if (string.IsNullOrEmpty(response.Content))
        {
            if (string.IsNullOrEmpty(response.ErrorMessage))
            {
                return new PluginApplicationException("Unknown error occurred. No content or error message provided. Status code: " + response.StatusCode);
            }
            
            return new PluginApplicationException($"{response.ErrorMessage}. Status code: {response.StatusCode}");
        }

        string? error;
        
        try
        {
            error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content!)!.Error;
        }
        catch (Exception ex)
        {
            if (response.Content.StartsWith("<"))
            {
                var doc = new HtmlDocument();
                doc.LoadHtml(response.Content);
                var parsedError = HtmlNodeToPlainText(doc.DocumentNode).Trim();
                error = $"Message: {response.ErrorMessage}. Content: {parsedError}";
            }
            else
            {
                error = $"Message: {response.ErrorMessage}. Content: {response.Content}";
            }
        }

        if (invocationContext?.Logger != null)
        {
            invocationContext?.Logger.LogError("Error from Sitecore: {Parameters}", [error]);
        }       

        throw new PluginApplicationException(error);
    }

    private static string HtmlNodeToPlainText(HtmlNode node)
    {
        if (node == null) return "";

        if (node.NodeType == HtmlNodeType.Text)
        {
            var text = ((HtmlTextNode)node).Text;
            return string.IsNullOrWhiteSpace(text) ? "" : text;
        }

        var result = "";
        foreach (var child in node.ChildNodes)
        {
            result += HtmlNodeToPlainText(child) + " ";
        }

        return result;
    }
}