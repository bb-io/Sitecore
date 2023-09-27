using Blackbird.Applications.Sdk.Common.Authentication;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sitecore
{
    public class SitecoreRequest : RestRequest
    {
        public SitecoreRequest(string endpoint, Method method,
            IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders) : base(endpoint, method)
        {
            Authenticate(authenticationCredentialsProviders);
        }

        public void Authenticate(IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders)
        {
            var client = new RestClient(authenticationCredentialsProviders.First(c => c.KeyName == "url").Value);
            var request = new RestRequest("/sitecore/api/ssc/auth/login", Method.Post);
            request.AddHeader("Content-Type", "application/json");
            request.AddJsonBody(new
            {
                domain = "sitecore",
                username = authenticationCredentialsProviders.First(c => c.KeyName == "username").Value,
                password = authenticationCredentialsProviders.First(c => c.KeyName == "password").Value
            });
            RestResponse response = client.Execute(request);
            //var cookieHeader = string.Join(';', response.Cookies.Select(c => $"{c.Name}={c.Value}"));
            foreach(var cookie in response.Cookies.ToList())
            {
                this.AddCookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain);
            }
        }
    }
}
