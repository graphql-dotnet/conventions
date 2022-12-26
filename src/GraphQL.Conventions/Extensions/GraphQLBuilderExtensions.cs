#nullable enable

using System;
using System.Collections.Generic;
using GraphQL.DI;

namespace GraphQL.Conventions
{
    public static class GraphQLBuilderExtensions
    {
        public static IGraphQLBuilder AddConventionsSchema(this IGraphQLBuilder builder, Func<GraphQLEngine> factory)
            => AddConventionsSchema(builder, _ => factory());

        public static IGraphQLBuilder AddConventionsSchema(this IGraphQLBuilder builder, Func<IServiceProvider, GraphQLEngine> factory)
        {
            return builder.AddSchema(provider =>
            {
                var engine = factory(provider);
                var schema = engine.GetSchema();
                var schemaConfigurations = provider.GetService<IEnumerable<IConfigureSchema>>();
                if (schemaConfigurations != null)
                {
                    foreach (var config in schemaConfigurations)
                    {
                        config.Configure(schema, provider);
                    }
                }
                return schema;
            });
        }

        public static IGraphQLBuilder AddConventionsSchema(this IGraphQLBuilder builder, Action<GraphQLEngine> configure)
            => AddConventionsSchema(builder, (engine, _) => configure.Invoke(engine));

        public static IGraphQLBuilder AddConventionsSchema(this IGraphQLBuilder builder, Action<GraphQLEngine, IServiceProvider> configure)
            => AddConventionsSchema(builder, provider =>
            {
                var engine = GraphQLEngine.New();
                configure(engine, provider);
                return engine;
            });

        public static IGraphQLBuilder AddConventionsSchema<TQuery>(this IGraphQLBuilder builder, Action<GraphQLEngine>? configure = null)
            => AddConventionsSchema<TQuery>(builder, (engine, _) => configure?.Invoke(engine));

        public static IGraphQLBuilder AddConventionsSchema<TQuery>(this IGraphQLBuilder builder, Action<GraphQLEngine, IServiceProvider> configure)
            => AddConventionsSchema(builder, provider =>
            {
                var engine = GraphQLEngine.New<TQuery>();
                configure?.Invoke(engine, provider);
                return engine;
            });

        public static IGraphQLBuilder AddConventionsSchema<TQuery, TMutation>(this IGraphQLBuilder builder, Action<GraphQLEngine>? configure = null)
            => AddConventionsSchema<TQuery, TMutation>(builder, (engine, _) => configure?.Invoke(engine));

        public static IGraphQLBuilder AddConventionsSchema<TQuery, TMutation>(this IGraphQLBuilder builder, Action<GraphQLEngine, IServiceProvider> configure)
            => AddConventionsSchema(builder, provider =>
            {
                var engine = GraphQLEngine.New<TQuery, TMutation>();
                configure.Invoke(engine, provider);
                return engine;
            });
    }
}
