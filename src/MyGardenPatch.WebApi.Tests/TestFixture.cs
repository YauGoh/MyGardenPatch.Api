using DotNet.Testcontainers.Images;

namespace MyGardenPatch.WebApi.Tests;

public record TestFixtureState();

public class TestFixture : IAsyncLifetime
{
    private TestcontainerDatabase dbContainer;
    internal Mock<IEmailSender> MockEmailSender { get; }

    internal Mock<IFileStorage> MockFileStorage { get; }

    internal IConfiguration? Configuration { get; private set; }

    private Dictionary<string, object> state;

    private readonly string databaseName = Guid.NewGuid().ToString();

    public TestFixture()
    {
        state = new Dictionary<string, object>();

        TestcontainersSettings.ResourceReaperImage = new DockerImage("ghcr.io/yaugoh/ryuk:latest");

        dbContainer = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(
                new MsSqlTestcontainerConfiguration
                {
                    Password = "P@ssw0rd!2345",
                })
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

        MockEmailSender = new Mock<IEmailSender>();

        MockFileStorage = new Mock<IFileStorage>();
    }

    internal IAlbaHost? Sut { get; private set; }

    public async Task InitializeAsync()
    {
        await dbContainer.StartAsync();

        Sut = await AlbaHost.For<global::Program>(x =>
        {
            x.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IEmailSender>(s => MockEmailSender.Object);
                services.AddSingleton<IFileStorage>(s => MockFileStorage.Object);

                ReplaceWithTestContainerDb<LocalIdentityDbContext>(services);
                ReplaceWithTestContainerDb<MyGardenPatchDbContext>(services);
            });
        });

        await EnsureDbMigrated(Sut.Services);

        Configuration = Sut.Services.GetRequiredService<IConfiguration>();

        this.WithApiKey();
    }

    public async Task DisposeAsync()
    {
        await dbContainer.StopAsync();
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

    public async Task Command(string url, dynamic? command = null)
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

    public async Task Command(string url, dynamic? command, params (string Name, string Filename, string ContentType, string Content, Dictionary<string, string> Headers)[] files)
    {
        command = command ?? new { };

        await Scenario(
            _ =>
            {
                var builder = _.PostFiles();

                foreach (var file in files)
                {
                    builder.WithFile(file.Name, file.Filename, file.ContentType, file.Content, file.Headers);
                }

                builder
                    .WithCommand(command)
                    .To(url);
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

        services.AddDbContext<TDbContext>(options => options.UseSqlServer(dbContainer.ConnectionString));

    }

    private async Task EnsureDbMigrated(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var localIdentityDbContext = scope.ServiceProvider.GetRequiredService<LocalIdentityDbContext>();
        await localIdentityDbContext.Database.MigrateAsync();
       
        var myGardenDbContext = scope.ServiceProvider.GetRequiredService<MyGardenPatchDbContext>();
        await myGardenDbContext.Database.MigrateAsync();
    }
}
