using System;
using System.Collections.Generic;

namespace GraphQL.Conventions.Attributes
{
    public abstract class AttributeBase : Attribute, IAttribute
    {
        public const AttributeTargets Everywhere = Types | FieldsAndParameters;
        public const AttributeTargets Types = AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface | AttributeTargets.Enum;
        public const AttributeTargets Fields = AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method;
        public const AttributeTargets FieldsAndParameters = Fields | AttributeTargets.Parameter;
        public const AttributeTargets Parameters = AttributeTargets.Parameter;

        private readonly AttributeApplicationPhase _phase;

        protected AttributeBase(AttributeApplicationPhase? phase = null)
        {
            _phase = phase ?? AttributeApplicationPhase.MetaDataDerivation;
        }

        public AttributeApplicationPhase Phase => _phase;

        public int ApplicationOrder { get; set; }

        public List<IAttribute> AssociatedAttributes { get; private set; } = new List<IAttribute>();
    }
}
