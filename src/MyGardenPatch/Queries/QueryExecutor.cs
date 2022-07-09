using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MyGardenPatch.Queries
{
    public interface IQueryExecutor
    {
        Task<TResult> HandleAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>;
    }

    internal class QueryExecutor : IQueryExecutor
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryExecutor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> HandleAsync<TQuery, TResult>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResult>
        {
            var validator = _serviceProvider.GetRequiredService<IQueryValidator<TQuery>>() ?? throw new QueryValidatorNotFoundException<TQuery>();

            var result = await validator.ValidateAsync(query, cancellationToken);

            if (!result.IsValid)
                throw new InvalidQueryException<TQuery>(query, result.Errors);

            var handler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResult>>() ?? throw new QueryHandlerNotFoundException<TQuery>();

            return await handler.HandleAsync(query, cancellationToken);
        }
    }

    public static class QueryExecutorExtensions
    {
        private static Dictionary<Type, Type>? _lookup = null;

        public static Task<TResult> HandleAsync<TResult>(this IQueryExecutor executor, IQuery<TResult> query, CancellationToken cancellationToken = default)
        {
            var lookup = _lookup ?? (_lookup = ResolveQueryLookup());

            var handleAsync = executor.GetType().GetMethod(nameof(QueryExecutor.HandleAsync));

            if (handleAsync == null) throw new InvalidOperationException("");

            return (Task<TResult>)handleAsync.MakeGenericMethod(_lookup[typeof(TResult)], typeof(TResult)).Invoke(executor, new object?[] { query, cancellationToken })!;

        }

        private static Dictionary<Type, Type> ResolveQueryLookup()
        {
            return Assembly.GetExecutingAssembly()
               .GetTypes()
               .Where(t => !t.IsAbstract &&
                           IsQuery(t))
               .ToDictionary(t => GetResult(t), t => t);
        }

        private static bool IsQuery(Type type) =>
            type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>));

        private static Type GetResult(Type type) =>
            type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQuery<>)).GetGenericArguments()[0];
    }
}
