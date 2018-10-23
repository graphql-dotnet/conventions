using System;
using GraphQL.Conventions;

namespace DataLoaderWithEFCore.GraphApi.Schema
{
    [InputType]
    public class UpdateMovieTitleParams
    {
        public Guid Id { get; set; }

        public string NewTitle { get; set; }
    }
}
