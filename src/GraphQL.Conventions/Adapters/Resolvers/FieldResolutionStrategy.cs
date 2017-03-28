using System;

namespace GraphQL.Conventions
{
    public enum FieldResolutionStrategy
    {
        Normal,
        WrappedSynchronous,
        WrappedAsynchronous,
    }
}