using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Apps.Sitecore
{
    public class SitecoreClient : RestClient
    {
        public SitecoreClient(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) : 
            base(new RestClientOptions() { ThrowOnAnyError = true, 
                BaseUrl = new Uri(authenticationCredentialsProviders.First(c => c.KeyName == "url").Value) }) 
        {
        }

        
    }
}
