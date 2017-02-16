using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphQL.Conventions.Attributes.Collectors
{
    public class AttributeCollector
    {
        public static Type ApplicationType { get; set; }
    }

    public class AttributeCollector<TAttribute>
        where TAttribute : IAttribute
    {
        class WrappedAttribute : IComparable<WrappedAttribute>
        {
            private static long _currentIndex = 0;

            public WrappedAttribute(TAttribute attribute)
            {
                InternalOrder = ++_currentIndex;
                Attribute = attribute;
                foreach (var associatedAttribute in Attribute.AssociatedAttributes
                    .Where(associatedAttribute => associatedAttribute is TAttribute))
                {
                    AssociatedAttributes.Add(new WrappedAttribute((TAttribute)associatedAttribute));
                }
            }

            public long InternalOrder { get; private set; }

            public TAttribute Attribute { get; private set; }

            public List<WrappedAttribute> AssociatedAttributes { get; private set; } = new List<WrappedAttribute>();

            public int CompareTo(WrappedAttribute other)
            {
                if (Attribute.Phase != other.Attribute.Phase)
                {
                    return (int)Attribute.Phase - (int)other.Attribute.Phase;
                }
                if (Attribute.ApplicationOrder != other.Attribute.ApplicationOrder)
                {
                    return (int)(Attribute.ApplicationOrder - other.Attribute.ApplicationOrder);
                }
                return (int)(InternalOrder - other.InternalOrder);
            }
        }

        private List<TAttribute> _defaultAttributes;

        public AttributeCollector()
        {
            DiscoverDefaultAttributes();
            if (AttributeCollector.ApplicationType != null)
            {
                DiscoverDefaultAttributes(AttributeCollector.ApplicationType);
            }
        }

        public List<TAttribute> CollectAttributes(ICustomAttributeProvider obj)
        {
            return CollectAttributesImplementation(obj)
                .OrderBy(attribute => attribute)
                .Select(wrappedAttribute => wrappedAttribute.Attribute)
                .ToList();
        }

        protected void AddDefaultAttributes(params TAttribute[] defaultAttributes)
        {
            if (_defaultAttributes == null)
            {
                _defaultAttributes = new List<TAttribute>();
            }
            _defaultAttributes.AddRange(defaultAttributes);
        }

        protected virtual IEnumerable<TAttribute> CollectCoreAttributes(ICustomAttributeProvider obj)
        {
            return new TAttribute[0];
        }

        private IEnumerable<WrappedAttribute> CollectAttributesImplementation(ICustomAttributeProvider obj) =>
            FlattenAttributeCollection(CollectCoreAttributes(obj))
                .Union(FlattenAttributeCollection(_defaultAttributes))
                .Union(GetAttributes(obj));

        private void DiscoverDefaultAttributes(Type assemblyType = null)
        {
            if (assemblyType == null)
            {
                assemblyType = typeof(IDefaultAttribute);
            }

            var assembly = assemblyType.GetTypeInfo().Assembly;
            var defaultAttributes = assembly
                .GetTypes()
                .Where(type => IsDefaultAttribute(type) && IsTAttribute(type))
                .Select(type => (TAttribute)Activator.CreateInstance(type))
                .ToArray();
            AddDefaultAttributes(defaultAttributes);
        }

        private void DiscoverDefaultAttributes()
        {
            if (_defaultAttributes == null)
            {
                DiscoverDefaultAttributes(null);
            }
        }

        private bool IsDefaultAttribute(Type type)
        {
            return type.GetTypeInfo()
                .GetInterfaces()
                .Any(iface => iface == typeof(IDefaultAttribute));
        }

        private bool IsTAttribute(Type type)
        {
            return type.GetTypeInfo()
                .GetInterfaces()
                .Any(iface => iface == typeof(TAttribute));
        }

        private static IEnumerable<WrappedAttribute> GetAttributes(ICustomAttributeProvider obj)
        {
            var memberInfo = obj as MemberInfo;
            if (memberInfo != null)
            {
                var attributes = memberInfo.GetCustomAttributes(true).OfType<TAttribute>();
                return FlattenAttributeCollection(attributes);
            }
            else
            {
                var attributes = obj?.GetCustomAttributes(typeof(TAttribute), true).OfType<TAttribute>();
                return FlattenAttributeCollection(attributes);
            }
        }

        private static IEnumerable<WrappedAttribute> FlattenAttributeCollection(IEnumerable<TAttribute> attributes) =>
            attributes
                .Reverse()
                .Select(attribute => new WrappedAttribute(attribute))
                .SelectMany(wrappedAttribute => AllAttributes(wrappedAttribute))
                ?? new WrappedAttribute[0];

        private static IEnumerable<WrappedAttribute> AllAttributes(WrappedAttribute attribute) =>
            new[] { attribute }.Union(attribute.AssociatedAttributes);
    }
}
