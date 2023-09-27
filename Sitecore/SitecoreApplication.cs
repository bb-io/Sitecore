using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication.OAuth2;
using System.Xml.Linq;

namespace Apps.Sitecore
{
    public class SitecoreApplication : IApplication
    {
        public string Name
        {
            get => "Sitecore";
            set { }
        }
        private readonly Dictionary<Type, object> _typesInstances;

        public SitecoreApplication()
        {
        }

        public T GetInstance<T>()
        {
            throw new NotImplementedException();
        }
    }
}
