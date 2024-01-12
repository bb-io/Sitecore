﻿using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.Extensions.System;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;
using Sitecore.Constants;
using Sitecore.Models.Responses;

namespace Sitecore.Api;

public class SitecoreClient : BlackBirdRestClient
{
    public SitecoreClient(IEnumerable<AuthenticationCredentialsProvider> creds) :
        base(new()
        {
            BaseUrl = creds.Get(CredsNames.Url).Value.ToUri().Append("api/blackbird")
        })
    {
    }

    public async Task<IEnumerable<T>> Paginate<T>(RestRequest request)
    {
        var page = 1;
        var baseUrl = request.Resource;

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
        var error = JsonConvert.DeserializeObject<ErrorResponse>(response.Content!)!;
        return new(error.Error);
    }
}