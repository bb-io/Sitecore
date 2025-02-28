﻿using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Sitecore;

public class SitecoreApplication : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.Cms];
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