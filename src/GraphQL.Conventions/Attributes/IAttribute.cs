using System.Collections.Generic;

namespace GraphQL.Conventions.Attributes
{
    public interface IAttribute
    {
        AttributeApplicationPhase Phase { get; }

        int ApplicationOrder { get; }

        List<IAttribute> AssociatedAttributes { get; }
    }
}
