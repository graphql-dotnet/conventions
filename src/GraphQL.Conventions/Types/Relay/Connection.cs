using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Attributes.MetaData;

namespace GraphQL.Conventions.Types.Relay
{
    [Description("Connection to related objects with relevant pagination information.")]
    public class Connection<T>
    {
        [Description(
            "A count of the total number of objects in this connection, ignoring pagination. This allows a " +
            "client to fetch the first five objects by passing \"5\" as the argument to first, then fetch " +
            "the total count so it could display \"5 of 83\", for example. In cases where we employ infinite " +
            "scrolling or don't have an exact count of entries, this field will return `null`.")]
        public int? TotalCount { get; set; }

        [Description("Information to aid in pagination.")]
        public NonNull<PageInfo> PageInfo { get; set; }

        [Description("Information to aid in pagination.")]
        public IEnumerable<Edge<T>> Edges { get; set; }

        [Description(
            "A list of all of the objects returned in the connection. This is a convenience field provided " +
            "for quickly exploring the API; rather than querying for `{ edges { node } }` when no edge data " +
            "is needed, this field can be used instead. Note that when clients like Relay need to fetch the " +
            "`cursor` field on the edge to enable efficient pagination, this shortcut cannot be used, and " +
            "the full `{ edges { node } }` version should be used instead.")]
        public IEnumerable<T> Items => Edges?.Select(edge => edge != null ? edge.Node : default(T));
    }
}
