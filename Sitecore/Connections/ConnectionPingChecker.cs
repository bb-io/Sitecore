using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Sitecore.Connections
{
    public class ConnectionPingChecker : IConnectionValidator
    {
        public ValueTask<ConnectionValidationResponse> ValidateConnection(IEnumerable<AuthenticationCredentialsProvider> authProviders, CancellationToken cancellationToken)
        {
            try
            {
                new SitecoreRequest("", Method.Get, authProviders);
                return new ValueTask<ConnectionValidationResponse>(new ConnectionValidationResponse()
                {
                    IsValid = true,
                    Message = "Success"
                });
            }
            catch (Exception ex)
            {
                return new ValueTask<ConnectionValidationResponse>(new ConnectionValidationResponse()
                {
                    IsValid = false,
                    Message = "Error! Invalid credentials"
                });
            }
        }
    }
}
