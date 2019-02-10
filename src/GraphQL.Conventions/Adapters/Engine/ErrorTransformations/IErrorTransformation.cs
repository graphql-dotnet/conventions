namespace GraphQL.Conventions.Adapters.Engine.ErrorTransformations
{
    public interface IErrorTransformation
    {
        ExecutionErrors Transform(ExecutionErrors errors);
    }
}
