using Apps.Sitecore.Constants;
using Apps.Sitecore.Models.Responses;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Blackbird.Applications.Sdk.Utils.RestSharp;
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
        string? error;
        try
        {
            error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content!)!.Error;            
        } catch(Exception ex)
        {
            error = $"Message: {response.ErrorMessage}. Content: {response.Content}";
        }

        if (invocationContext?.Logger != null)
        {
            invocationContext?.Logger.LogError("Error from Sitecore: {Parameters}", [error]);
        }        
        throw new PluginApplicationException(error);
    }
}