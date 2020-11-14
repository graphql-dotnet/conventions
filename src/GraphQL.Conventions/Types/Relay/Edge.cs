// ReSharper disable once CheckNamespace
namespace GraphQL.Conventions.Relay
{
    [Description("An edge in a connection between two objects.")]
    public class Edge<T>
    {
        [Description("A cursor for use in pagination.")]
        public Cursor Cursor { get; set; }

        [Description("The item at the end of the edge.")]
        public T Node { get; set; }
    }
}
