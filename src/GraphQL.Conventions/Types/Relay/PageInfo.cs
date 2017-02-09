namespace GraphQL.Conventions.Relay
{
    [Description("Information about pagination in a connection.")]
    public class PageInfo
    {
        [Description("When paginating forwards, are there more items?")]
        public bool HasNextPage { get; set; }

        [Description("When paginating backwards, are there more items?")]
        public bool HasPreviousPage { get; set; }

        [Description("When paginating backwards, the cursor to continue.")]
        public Cursor StartCursor { get; set; }

        [Description("When paginating forwards, the cursor to continue.")]
        public Cursor EndCursor { get; set; }
    }
}
