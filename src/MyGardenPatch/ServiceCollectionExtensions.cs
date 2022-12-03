using MyGardenPatch.Configurations;

namespace MyGardenPatch;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMyGardenPatch(this IServiceCollection services, IConfiguration config)
    {
        services.AddScoped<IDomainEventBus, InMemoryDomainEventBus>();
        services.AddScoped<IFileAttachments, InMemoryFileAttachments>();

#if DEBUG
        services.AddScoped<IFileStorage, FileSystemFileStorage>();
#else
        services.Configure<AzureBlobStorageConfig>(config.GetSection("AzureBlob"));
        services.AddScoped<IFileStorage, AzureBlobFileStorage>();
#endif

        services.AddScoped<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.Configure<EmailConfig>(config.GetSection("Email"));
        services.Configure<FrontEndConfig>(config.GetSection("FrontEnd"));
        

        RegisterQueries(services);

        RegisterCommands(services);

        RegisterDomainEventHandlers(services);

        return services;
    }

    private static void RegisterCommands(IServiceCollection services)
    {
        services.AddScoped<ICommandExecutor, CommandExecutor>();

        var assembly = Assembly.GetExecutingAssembly();

        var commandHandlers = assembly
            .GetTypes()
            .Where(handler => !handler.IsAbstract &&
                              IsCommandHandler(handler))
            .SelectMany(
                handler => GetCommandTypeFromHandler(handler),
                (handler, command) => (handler, command))
            .ToList();

        foreach (var (handler, command) in commandHandlers)
        {
            var @interface = typeof(ICommandHandler<>).MakeGenericType(command);

            services.AddScoped(@interface, handler);
        }

        var commandValidators = assembly
            .GetTypes()
            .Where(validator => !validator.IsAbstract &&
                                IsCommandValidator(validator))
            .SelectMany(
                validator => GetCommandTypeFromValidator(validator),
                (validator, command) => (validator, command))
            .ToList();

        foreach (var (validator, command) in commandValidators)
        {
            var @interface = typeof(ICommandValidator<>).MakeGenericType(command);

            services.AddScoped(@interface, validator);
        }
    }

    private static void RegisterQueries(IServiceCollection services)
    {
        services.AddScoped<IQueryExecutor, QueryExecutor>();

        var assembly = Assembly.GetExecutingAssembly();

        var queryHandlerInfos = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract &&
                        IsQueryHandler(t))
            .SelectMany(
                handler => GetQueryHandlerTypes(handler),
                (handler, args) => (handler, args.query, args.result))
            .ToList();

        foreach (var (handler, query, result) in queryHandlerInfos)
        {
            var @interface = typeof(IQueryHandler<,>).MakeGenericType(query, result);

            services.AddScoped(@interface, handler);
        }

        var queryValidators = assembly
            .GetTypes()
            .Where(t => !t.IsAbstract &&
                        IsQueryValidator(t))
            .SelectMany(
                validator => GetQueryTypeFromValidator(validator),
                (validator, query) => (validator, query))
            .ToList();

        foreach (var (validator, query) in queryValidators)
        {
            var @interface = typeof(IQueryValidator<>).MakeGenericType(query);

            services.AddScoped(@interface, validator);
        }
    }

    private static bool IsCommandHandler(Type type) =>
        type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

    private static IEnumerable<Type> GetCommandTypeFromHandler(Type type)
    {
        var handlers = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<>));

        return handlers.Select(h => h.GetGenericArguments()[0]);
    }

    private static bool IsCommandValidator(Type type) =>
        type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandValidator<>));

    private static IEnumerable<Type> GetCommandTypeFromValidator(Type type)
    {
        var handlers = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandValidator<>));

        return handlers.Select(h => h.GetGenericArguments()[0]);
    }

    private static bool IsQueryHandler(Type type) =>
        type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));


    private static IEnumerable<(Type query, Type result)> GetQueryHandlerTypes(Type type)
    {
        var queries = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

        return queries.Select(q =>
        {
            var genericArgs = q.GetGenericArguments();

            return (genericArgs[0], genericArgs[1]);
        });
    }

    private static bool IsQueryValidator(Type type) =>
        type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryValidator<>));

    private static IEnumerable<Type> GetQueryTypeFromValidator(Type type)
    {
        var handlers = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IQueryValidator<>));

        return handlers.Select(h => h.GetGenericArguments()[0]);
    }

    private static void RegisterDomainEventHandlers(IServiceCollection services)
    {
        var domainHandlers = DomainEventHandlers.DiscoverAll();

        foreach (var info in domainHandlers)
            services.AddScoped(info.Interface, info.Handler);
    }
}
