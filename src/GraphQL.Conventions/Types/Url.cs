using System;

// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions
{
    public class Url : Uri
    {
        public Url(string url)
            : base(url)
        {
            if (!IsAbsoluteUri || HostNameType == UriHostNameType.Unknown)
            {
                throw new ArgumentException($"Invalid URL \"{url}\".");
            }
        }
    }
}
