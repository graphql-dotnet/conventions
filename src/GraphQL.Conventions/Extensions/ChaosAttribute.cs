using System;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Conventions;
using GraphQL.Conventions.Attributes;
using GraphQL.Conventions.Attributes.Collectors;
using GraphQL.Conventions.Execution;
using GraphQL.Conventions.Types.Descriptors;

namespace GraphQL.Conventions.Extensions
{
    public class ChaosAttribute : ExecutionFilterAttributeBase
    {
        public static bool IsEnabled = false;

        private const int DefaultSuccessRate = 50;

        private static readonly Random _random = new Random();

        private readonly int _successRate;

        public ChaosAttribute(int successRate = DefaultSuccessRate)
        {
            _successRate = successRate;
        }

        public override Task<object> Execute(IResolutionContext context, FieldResolutionDelegate next)
        {
            if (_random.Next(0, 100) > _successRate)
            {
                var path = $"{context.FieldInfo.DeclaringType.Name}.{context.FieldInfo.Name}";
                throw new ChaosException($"Only {_successRate} % of requests will succeed.", path);
            }
            return next(context);
        }
    }

    public class ChaosMetaDataAttribute : MetaDataAttributeBase, IDefaultAttribute
    {
        public override void MapField(GraphFieldInfo entity, MemberInfo memberInfo)
        {
            if (ChaosAttribute.IsEnabled)
            {
                entity.ExecutionFilters.Add(new ChaosAttribute());
            }
        }
    }

    public class ChaosException : Exception
    {
        public ChaosException(string message, string path)
            : base(message)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}
