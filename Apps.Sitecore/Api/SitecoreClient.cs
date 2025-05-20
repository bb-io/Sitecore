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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Apps.Sitecore.Api;

public class SitecoreClient : BlackBirdRestClient
{
    private const int PaginationStepSize = 20;
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
        var page = 1;
        var baseUrl = request.Resource.SetQueryParameter("pageSize", PaginationStepSize.ToString());

        var result = new List<T>();
        T[] response;
        do
        {
            request.Resource = baseUrl.SetQueryParameter("page", page++.ToString());
            response = await ExecuteWithErrorHandling<T[]>(request);

            result.AddRange(response);
        } while (response.Any());

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

        if (invocationContext.Logger != null)
        {
            invocationContext.Logger.LogError("Error from Sitecore: {Parameters}", [error]);
        }        
        throw new PluginApplicationException(error);
    }
}