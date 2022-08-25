namespace MyGardenPatch.Tests;

public class TestBase
{
    private IServiceProvider _serviceProvider;
    private string _inmemoryDatabaseName = Guid.NewGuid().ToString();

    protected Mock<IDomainEventBus> MockDomainEventBus { get; private set; }

    protected Mock<ICurrentUserProvider> MockCurrentUserProvider { get; private set; }

    protected Mock<IDateTimeProvider> MockDateTimeProvider { get; private set; }

    protected Mock<IEmailSender> MockEmailSender { get; private set; }

    protected Mock<IHttpContextAccessor> MockHttpContextAccessor { get; private set; }

    protected EmailAssertions EmailAssertions { get; private set; }

    protected HttpContext HttpContext { get; private set; }

    public TestBase()
    {
        var services = new ServiceCollection();
        var mockConfiguration = new Mock<IConfiguration>();
        var mockEmailConfig = new Mock<IOptions<EmailConfig>>();
        mockEmailConfig.Setup(c => c.Value).Returns(new EmailConfig("0.0.0.0", 0, new EmailAddress("testing@email.com", "testing")));

        mockConfiguration
            .Setup(c => c.GetSection(nameof(Email)))
            .Returns(new Mock<IConfigurationSection>().Object);

        services.AddTransient<IOptions<EmailConfig>>(src => mockEmailConfig.Object);

        var mockFrontEndConfig = new Mock<IOptions<FrontEndConfig>>();
        mockFrontEndConfig.Setup(c => c.Value).Returns(new FrontEndConfig());

        mockConfiguration
            .Setup(c => c.GetSection("FrontEnd"))
            .Returns(new Mock<IConfigurationSection>().Object);

        services.AddTransient<IOptions<FrontEndConfig>>(src => mockFrontEndConfig.Object);


        services.AddMyGardenPatchWebApi(mockConfiguration.Object);

        ReplaceMyVegePatchDbContextWithInmemoryDatabase(services);
        ReplaceLocalIdentityDbContextWithInmemoryDatabase(services);

        MockDomainEventBus = ReplaceWithMockBus(services);

        MockCurrentUserProvider = ReplaceWithMockUserProvider(services);

        MockDateTimeProvider = ReplaceWithMock<IDateTimeProvider>(services);

        MockEmailSender = ReplaceWithMock<IEmailSender>(services);

        EmailAssertions = new EmailAssertions(MockEmailSender);

        HttpContext = new DefaultHttpContext();

        MockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        MockHttpContextAccessor.Setup(a => a.HttpContext)
            .Returns(HttpContext);

        services.AddSingleton<IHttpContextAccessor>(src => MockHttpContextAccessor.Object);

        _serviceProvider = services.BuildServiceProvider();

        EnsureSeedData();

        HttpContext.RequestServices = _serviceProvider;
    }

    protected async Task ExecuteCommandAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var commandExecutor = GetService<ICommandExecutor>();

        await commandExecutor.HandleAsync(command);
    }

    protected async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
    {
        var executor = GetService<IQueryExecutor>();

        return await executor.HandleAsync(query);
    }

    protected async Task<TAggregate?> GetAsync<TAggregate, TAggregateId>(TAggregateId id)
        where TAggregate : Aggregate<TAggregateId>
        where TAggregateId : struct, IEquatable<TAggregateId>, IEntityId
    {
        var repository = GetService<IRepository<TAggregate, TAggregateId>>();

        return await repository.GetAsync(id);
    }

    protected async Task<TAggregate?> GetAsync<TAggregate, TAggregateId>(Expression<Func<TAggregate, bool>> expression)
        where TAggregate : Aggregate<TAggregateId>
        where TAggregateId : struct, IEquatable<TAggregateId>, IEntityId
    {
        var repository = GetService<IRepository<TAggregate, TAggregateId>>();

        return await repository.GetByExpressionAsync(expression);
    }

    protected async Task<bool> AnyAsync<TAggregate, TAggregateId>(Expression<Func<TAggregate, bool>> expression)
        where TAggregate : Aggregate<TAggregateId>
        where TAggregateId : struct, IEquatable<TAggregateId>, IEntityId
    {
        var repository = GetService<IRepository<TAggregate, TAggregateId>>();

        return await repository.AnyAsync(expression);
    }

    protected Task<T?> GetIdentityAsync<T>(Expression<Func<T, bool>> expression) where T : class
    {
        var dbContext = GetService<LocalIdentityDbContext>();

        return dbContext.Set<T>().SingleOrDefaultAsync(expression);
    }

    protected TService GetService<TService>() where TService : notnull => _serviceProvider.GetRequiredService<TService>();

    protected void SetCurrentUser(UserId? userId)
    {
        MockCurrentUserProvider
            .Setup(_ => _.CurrentUserId)
            .Returns(userId);
    }

    protected void SetCurrentUser(User user)
    {
        MockCurrentUserProvider
            .Setup(_ => _.CurrentUserId)
            .Returns(user.Id);

        MockCurrentUserProvider
            .Setup(_ => _.CurrentEmailAddress)
            .Returns(user.EmailAddress);
    }

    protected TestBase SeedWith<TAggregate>(params TAggregate[] aggregates)
        where TAggregate : class
    {
        var dbContext = GetService<MyGardenPatchDbContext>();

        dbContext.AddRange(aggregates);

        dbContext.SaveChanges();

        return this;
    }

    private void ReplaceMyVegePatchDbContextWithInmemoryDatabase(IServiceCollection services)
    {
        var serviceDescriptions = services.Where(s => s.ServiceType == typeof(MyGardenPatchDbContext) ||
                                                      s.ServiceType == typeof(DbContextOptions) ||
                                                      s.ServiceType == typeof(DbContextOptions<MyGardenPatchDbContext>)).ToList();
        foreach (var serviceDescription in serviceDescriptions)
        {
            services.Remove(serviceDescription);
        }

        services.AddDbContext<MyGardenPatchDbContext>(options => options.UseInMemoryDatabase(_inmemoryDatabaseName));
    }

    private void ReplaceLocalIdentityDbContextWithInmemoryDatabase(IServiceCollection services)
    {
        var serviceDescriptions = services.Where(s => s.ServiceType == typeof(LocalIdentityDbContext) ||
                                                      s.ServiceType == typeof(DbContextOptions) ||
                                                      s.ServiceType == typeof(DbContextOptions<LocalIdentityDbContext>)).ToList();
        foreach (var serviceDescription in serviceDescriptions)
        {
            services.Remove(serviceDescription);
        }

        services.AddDbContext<LocalIdentityDbContext>(options => options.UseInMemoryDatabase(_inmemoryDatabaseName));
    }

    private Mock<IDomainEventBus> ReplaceWithMockBus(ServiceCollection services)
    {
        var serviceDescriptions = services.Where(s => s.ServiceType == typeof(IDomainEventBus)).ToList();
        foreach (var serviceDescription in serviceDescriptions)
        {
            services.Remove(serviceDescription);
        }

        var mock = new Mock<IDomainEventBus>();

        services.AddSingleton(_ => mock.Object);

        return mock;
    }

    private Mock<ICurrentUserProvider> ReplaceWithMockUserProvider(ServiceCollection services)
    {
        var serviceDescriptions = services.Where(s => s.ServiceType == typeof(ICurrentUserProvider)).ToList();
        foreach (var serviceDescription in serviceDescriptions)
        {
            services.Remove(serviceDescription);
        }

        var mock = new Mock<ICurrentUserProvider>();

        services.AddSingleton(_ => mock.Object);

        return mock;
    }

    private Mock<T> ReplaceWithMock<T>(ServiceCollection services) where T : class
    {
        var serviceDescriptions = services.Where(s => s.ServiceType == typeof(T)).ToList();
        foreach (var serviceDescription in serviceDescriptions)
        {
            services.Remove(serviceDescription);
        }

        var mock = new Mock<T>();

        services.AddSingleton<T>(_ => mock.Object);

        return mock;
    }

    private void EnsureSeedData()
    {
        _serviceProvider.GetRequiredService<LocalIdentityDbContext>().Database.EnsureCreated();
        _serviceProvider.GetRequiredService<MyGardenPatchDbContext>().Database.EnsureCreated();
    }
}