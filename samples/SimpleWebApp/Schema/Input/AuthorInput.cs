namespace GraphQL.Conventions.Tests.Server.Schema.Input
{
    [InputType]
    [Description("An author")]
    public class AuthorInput
    {
        [Description("The author's firstname.")]
        public string FirstName { get; set; }

        [Description("The author's lastname.")]
        public NonNull<string> LastName { get; set; }
    }
}
