using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using MyGardenPatch.Webapi;
using MyGardenPatch.WebApiExtensions;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging
    .AddSerilog(logger)
    .AddConsole();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // = true;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMicrosoftIdentityWebApiAuthentication(builder.Configuration);
builder.Services.AddMyGardenPatchWebApi(builder.Configuration);
builder.Services.AddAuthentication();
builder.Services.AddRoleBasedAuthorization();

var allowedOrigins = builder.Configuration.GetSection($"Cors:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(
    options => options.AddPolicy(
        "WebApp",
        policy => policy
            .WithOrigins(allowedOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Development CDN to server locally attached images and files
#if DEBUG

app.UseFileServer(new FileServerOptions { 
    RequestPath = "/images",
    FileProvider = new PhysicalFileProvider(builder.Configuration.GetValue<string>("StorageFolder")),
    EnableDirectoryBrowsing = true
    
});

#endif

app.UseAuthentication();
app.UseAuthorization();

app.UseCors("WebApp");

app.AddCommandsAndQueries("WebApp");

app.Run();
