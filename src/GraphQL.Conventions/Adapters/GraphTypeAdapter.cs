using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Conventions.Types.Descriptors;
using GraphQL.Conventions.Types.Resolution.Extensions;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.Conventions.Adapters
{
    public class GraphTypeAdapter : IGraphTypeAdapter<ISchema, IGraphType>
    {
        private readonly CachedRegistry<Type, IGraphType> _typeDescriptors = new CachedRegistry<Type, IGraphType>();

        private readonly Dictionary<string, Type> _registeredScalarTypes = new Dictionary<string, Type>();

        public Func<GraphFieldInfo, IFieldResolver> FieldResolverFactory { get; set; } = (fieldInfo) => new FieldResolver(fieldInfo);

        public ISchema DeriveSchema(GraphSchemaInfo schemaInfo)
        {
            var types = schemaInfo
                .Types
                .Where(t => !t.IsIgnored && !t.Interfaces.Any(iface => iface.IsIgnored))
                .ToList();
            foreach (var type in types)
            {
                DeriveType(type);
            }
            var interfaces = types
                .Where(t => t.IsInterfaceType && !t.IsInputType && t.PossibleTypes.Any())
                .GroupBy(t => t.Name)
                .Select(g => g.First());
            var possibleTypes = interfaces
                .Where(t => !t.IsIgnored)
                .SelectMany(t => t.PossibleTypes)
                .Select(typeInfo => (typeInfo.IsIgnored || !typeInfo.IsNullable || typeInfo.Interfaces.Any(x => x.IsIgnored == true))
                    ? null
                    : DeriveType(typeInfo)
                )
                .Where(x => x != null)
                .GroupBy(t => t.Name)
                .Select(g => g.First())
                .ToArray();
            var schema = new Schema(new FuncDependencyResolver(DeriveTypeFromTypeInfo))
            {
                Query = DeriveOperationType(schemaInfo.Query),
                Mutation = DeriveOperationType(schemaInfo.Mutation),
                Subscription = DeriveOperationType(schemaInfo.Subscription),
            };            
            schema.RegisterTypes(possibleTypes);
            return schema;
        }

        public IGraphType DeriveType(GraphTypeInfo typeInfo)
        {
            var primitiveType = GetPrimitiveType(typeInfo);
            var graphType = primitiveType != null
                ? WrapNonNullableType(typeInfo, Activator.CreateInstance(primitiveType) as GraphType)
                : _typeDescriptors.GetEntity(typeInfo.TypeRepresentation.AsType());
            return graphType ?? GetComplexType(typeInfo);
        }

        public void RegisterScalarType<TType>(string name)
        {
            _registeredScalarTypes.Add(name, typeof(TType));
        }

        private IGraphType DeriveTypeFromTypeInfo(Type type)
        {
            var graphType = _typeDescriptors.GetEntity(type);
            if (graphType != null)
            {
                return graphType;
            }
            return (IGraphType)Activator.CreateInstance(type);
        }

        private IObjectGraphType DeriveOperationType(GraphTypeInfo typeInfo) =>
            typeInfo != null
                ? DeriveType(typeInfo) as IObjectGraphType
                : null;

        private FieldType DeriveField(GraphFieldInfo fieldInfo)
        {
            if (fieldInfo.Type.IsObservable)
            {
                var resolver = new Resolvers.EventStreamResolver(fieldInfo);
                return new EventStreamFieldType
                {
                    Name = fieldInfo.Name,
                    Description = fieldInfo.Description,
                    DeprecationReason = fieldInfo.DeprecationReason,
                    DefaultValue = fieldInfo.DefaultValue,
                    Type = GetType(fieldInfo.Type),
                    Arguments = new QueryArguments(fieldInfo.Arguments.Where(arg => !arg.IsInjected).Select(DeriveArgument)),
                    Resolver = resolver,
                    Subscriber = resolver
                };
            }
            return new FieldType
            {
                Name = fieldInfo.Name,
                Description = fieldInfo.Description,
                DeprecationReason = fieldInfo.DeprecationReason,
                DefaultValue = fieldInfo.DefaultValue,
                Type = GetType(fieldInfo.Type),
                Arguments = new QueryArguments(fieldInfo.Arguments.Where(arg => !arg.IsInjected).Select(DeriveArgument)),
                Resolver = FieldResolverFactory(fieldInfo),
            };
        }

        private QueryArgument DeriveArgument(GraphArgumentInfo argumentInfo)
        {
            return new QueryArgument(GetType(argumentInfo.Type))
            {
                Name = argumentInfo.Name,
                Description = argumentInfo.Description,
                DefaultValue = argumentInfo.DefaultValue,
            };
        }

        private Type GetType(GraphTypeInfo typeInfo) => DeriveType(typeInfo).GetType();

        private Type GetPrimitiveType(GraphTypeInfo typeInfo)
        {
            switch (typeInfo.Name)
            {
                case TypeNames.Integer:
                    return typeof(IntGraphType);

                case TypeNames.Float:
                    return typeof(FloatGraphType);

                case TypeNames.Decimal:
                    return typeof(DecimalGraphType);

                case TypeNames.String:
                    return typeof(StringGraphType);

                case TypeNames.Boolean:
                    return typeof(BooleanGraphType);

                case TypeNames.Date:
                    return typeof(DateGraphType);

                case TypeNames.DateTime:
                    return typeof(DateTimeGraphType);

                case TypeNames.DateTimeOffset:
                    return typeof(DateTimeOffsetGraphType);

                case TypeNames.Id:
                    return typeof(Types.IdGraphType);

                case TypeNames.Cursor:
                    return typeof(Types.Relay.CursorGraphType);

                case TypeNames.TimeSpan:
                    return typeof(Types.TimeSpanGraphType);

                case TypeNames.Url:
                    return typeof(Types.UrlGraphType);

                case TypeNames.Uri:
                    return typeof(Types.UriGraphType);

                case TypeNames.Guid:
                    return typeof(Types.GuidGraphType);

                default:
                    if (!string.IsNullOrWhiteSpace(typeInfo.Name) &&
                        _registeredScalarTypes.TryGetValue(typeInfo.Name, out var type))
                    {
                        return type;
                    }
                    return null;
            }
        }

        private IGraphType GetComplexType(GraphTypeInfo typeInfo)
        {
            if (typeInfo.IsInterfaceType)
            {
                return ConstructInterfaceType(typeInfo);
            }
            else if (typeInfo.IsEnumerationType)
            {
                return ConstructEnumerationType(typeInfo);
            }
            else if (typeInfo.IsUnionType)
            {
                return ConstructUnionType(typeInfo);
            }
            else if (typeInfo.IsListType)
            {
                return ConstructListType(typeInfo);
            }
            else if (typeInfo.IsInputType)
            {
                return ConstructInputType(typeInfo);
            }
            else if (typeInfo.IsOutputType)
            {
                return ConstructOutputType(typeInfo);
            }
            else
            {
                throw new ArgumentException(
                    $"Unable to derive graph type for '{typeInfo.TypeRepresentation.Name}'.");
            }
        }

        private IGraphType CacheType(GraphTypeInfo typeInfo, IGraphType graphType)
        {
            _typeDescriptors.AddEntity(typeInfo.TypeRepresentation.AsType(), WrapNonNullableType(typeInfo, graphType));
            _typeDescriptors.AddEntity(graphType.GetType(), graphType);
            return graphType;
        }

        private TType CreateTypeInstance<TType>(Type type)
        {
            var obj = Activator.CreateInstance(type);
            return obj is TType castType ? castType : default;
        }

        private IGraphType WrapNonNullableType(GraphTypeInfo typeInfo, IGraphType graphType) =>
            typeInfo.IsNullable
                ? graphType
                : CreateTypeInstance<GraphType>(typeof(NonNullGraphType<>).MakeGenericType(graphType.GetType()));

        private TReturnType ConstructType<TReturnType>(Type template, GraphTypeInfo typeInfo)
            where TReturnType : class, IGraphType
        {
            var typeParameter = typeInfo.GetTypeRepresentation();
            var type = CreateTypeInstance<TReturnType>(template.MakeGenericType(typeParameter.AsType()));
            if (type == null)
            {
                return default;
            }
            type.Name = typeInfo.Name;
            type.Description = typeInfo.Description;
            type.DeprecationReason = typeInfo.DeprecationReason;
            return CacheType(typeInfo, type) as TReturnType;
        }

        private IGraphType ConstructEnumerationType(GraphTypeInfo typeInfo)
        {
            var graphType = ConstructType<EnumerationGraphType>(typeof(Types.EnumerationGraphType<>), typeInfo);
            foreach (var value in typeInfo.Fields)
            {
                graphType.AddValue(value.Name, value.Description, value.Value, value.DeprecationReason);
            }
            return WrapNonNullableType(typeInfo, graphType);
        }

        private IGraphType ConstructInterfaceType(GraphTypeInfo typeInfo)
        {
            var graphType = ConstructType<IInterfaceGraphType>(typeof(Types.InterfaceGraphType<>), typeInfo);
            if (typeInfo.IsIgnored)
            {
                return WrapNonNullableType(typeInfo, graphType);
            }
            foreach (var possibleType in typeInfo
                .PossibleTypes
                .Select(t => DeriveType(t) as IObjectGraphType))
            {
                if (possibleType == null)
                {
                    continue;
                }
                graphType.AddPossibleType(possibleType);
            }
            DeriveFields(typeInfo, graphType);
            return WrapNonNullableType(typeInfo, graphType);
        }

        private IGraphType ConstructUnionType(GraphTypeInfo typeInfo)
        {
            var graphType = ConstructType<UnionGraphType>(typeof(Types.UnionGraphType<>), typeInfo);
            foreach (var possibleType in typeInfo.PossibleTypes.Select(t => DeriveType(t) as IObjectGraphType))
            {
                if (possibleType != null) graphType.Type(possibleType.GetType());
            }
            return WrapNonNullableType(typeInfo, graphType);
        }

        private IGraphType ConstructListType(GraphTypeInfo typeInfo)
        {
            var type = typeof(ListGraphType<>).MakeGenericType(GetType(typeInfo.TypeParameter));
            var graphType = CacheType(typeInfo, CreateTypeInstance<GraphType>(type));
            return WrapNonNullableType(typeInfo, graphType);
        }

        private IGraphType ConstructInputType(GraphTypeInfo typeInfo)
        {
            var graphType = ConstructType<InputObjectGraphType>(typeof(Types.InputObjectGraphType<>), typeInfo);
            DeriveFields(typeInfo, graphType);
            return WrapNonNullableType(typeInfo, graphType);
        }

        private IGraphType ConstructOutputType(GraphTypeInfo typeInfo)
        {
            var graphType = ConstructType<ObjectGraphType>(typeof(Types.OutputObjectGraphType<>), typeInfo);
            foreach (var iface in typeInfo.Interfaces.Select(GetType))
            {
                graphType.Interface(iface);
            }
            graphType.IsTypeOf = obj =>
            {
                var objType = UnwrapObject(obj)?.GetType().GetTypeRepresentation();
                var typeRepresentation = typeInfo.GetTypeRepresentation();
                return objType != null && (Equals(objType, typeRepresentation) || objType.IsSubclassOf(typeRepresentation.UnderlyingSystemType));
            };
            DeriveFields(typeInfo, graphType);
            return WrapNonNullableType(typeInfo, graphType);
        }

        private static object UnwrapObject(object obj)
        {
            if (obj is INonNull @null)
            {
                obj = @null.ObjectValue;
            }
            if (obj is Union union)
            {
                obj = union.Instance;
            }
            return obj;
        }

        private void DeriveFields(GraphTypeInfo typeInfo, IComplexGraphType graphType)
        {
            foreach (var field in typeInfo.Fields.Select(DeriveField))
            {
                graphType.AddField(field);
            }
        }
    }
}