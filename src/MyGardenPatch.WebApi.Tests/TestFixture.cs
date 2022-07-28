namespace MyGardenPatch.WebApi.Tests;

public record TestFixtureState();

public class TestFixture : IAsyncLifetime
{
    //private MsSqlTestcontainer dbContainer;
    internal Mock<IEmailSender> MockEmailSender { get; }
    internal IConfiguration Configuration { get; }

    private GardenId gardenId;
    private GardenBedId gardenBedId;
    private PlantId plantId;

    private Dictionary<string, object> state;

    private readonly string databaseName = Guid.NewGuid().ToString();

    public TestFixture()
    {
        state = new Dictionary<string, object>();

        //dbContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
        //    .WithDatabase<MsSqlTestcontainer>(
        //        new MsSqlTestcontainerConfiguration
        //        {
        //            Database = "my-garden-patch",
        //            Username = "user",
        //            Password = "password"
        //        })
        //    .Build();

        MockEmailSender = new Mock<IEmailSender>();

        Sut = AlbaHost.For<global::Program>(x =>
        {
            x.ConfigureServices((context, services) =>
            {
                services.AddTransient<IEmailSender>(s => MockEmailSender.Object);

               

                ReplaceWithTestContainerDb<LocalIdentityDbContext>(services);
                ReplaceWithTestContainerDb<MyGardenPatchDbContext>(services);
            });
        }).Result;

        Configuration = Sut.Services.GetRequiredService<IConfiguration>();

        this.WithApiKey();
    }

    internal IAlbaHost Sut { get; private set; }

    public async Task InitializeAsync()
    {
        //await dbContainer.StartAsync();
        await EnsureDbMigrated(Sut.Services);

    }

    public async Task DisposeAsync()
    {
        //await dbContainer.StopAsync();
    }

    public void SetState<T>(string key, T value) {
        if (!state.TryAdd(key, value))
            state[key] = value;
    }

    public void SetState<T>(T value) => SetState<T>(typeof(T).FullName!, value);

    public T? GetState<T>(string key)
    {
        if (state.TryGetValue(key, out var value))
        {
            return value.As<T>();
        }

        return default(T);
    }

    public T GetState<T>() => GetState<T>(typeof(T).FullName!);

    public async Task<IScenarioResult> Scenario(Action<Scenario> configure)
    {
        await this.Setup();

        return await this.WithAuth(configure);
    }

    public async Task Command(string url, dynamic command = null)
    {
        command = command ?? new { };

        await Scenario(
            _ =>
            {
                _.Post
                    .Json(command)
                    .ToUrl(url);

                _.StatusCodeShouldBeOk();
            });
    }

    public async Task<TResult> Query<TResult>(string url, dynamic? query = null)
    {
        query = query ?? new { };

        var response = await Scenario(
            _ =>
            {
                _.Post
                    .Json(query)
                    .ToUrl(url);

                _.StatusCodeShouldBeOk();
            });

        var result = response
            .ReadAsJson<TResult>()!;

        return result;
    }

    private void ReplaceWithTestContainerDb<TDbContext>(IServiceCollection services) where TDbContext : DbContext
    {

        var serviceDescriptions = services.Where(s => s.ServiceType == typeof(TDbContext) ||
                                             s.ServiceType == typeof(DbContextOptions<TDbContext>)).ToList();
        foreach (var serviceDescription in serviceDescriptions)
        {
            services.Remove(serviceDescription);
        }

        services.AddDbContext<TDbContext>(options => options.UseInMemoryDatabase(databaseName));

    }

    private async Task EnsureDbMigrated(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var localIdentityDbContext = scope.ServiceProvider.GetRequiredService<LocalIdentityDbContext>();
        await localIdentityDbContext.Database.EnsureCreatedAsync();

       
        var myGardenDbContext = scope.ServiceProvider.GetRequiredService<MyGardenPatchDbContext>();
        await myGardenDbContext.Database.EnsureCreatedAsync();
    }

    

    
}
