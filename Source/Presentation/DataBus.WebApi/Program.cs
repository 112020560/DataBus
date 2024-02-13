using Asp.Versioning;
using DataBus.WebApi;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Net.Http.Headers;
using Serilog;
using DataBus.Application;

var corsPolicy = "CORSPolicy";
var version1 = new ApiVersion(1);
var version2 = new ApiVersion(2);
var version3 = new ApiVersion(3);


var builder = WebApplication.CreateBuilder(args);

//logger configuration
var logger = new LoggerConfiguration()
  .ReadFrom.Configuration(builder.Configuration)
  .Enrich.FromLogContext()
  .CreateLogger();

logger.Information($"Environmet : {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");

builder.Host.UseSerilog(logger);

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.ResponsePropertiesAndHeaders |
                            HttpLoggingFields.ResponseBody |
                            HttpLoggingFields.RequestPropertiesAndHeaders |
                            HttpLoggingFields.RequestBody;

    options.RequestHeaders.Add(HeaderNames.Accept);
    options.RequestHeaders.Add(HeaderNames.ContentType);
    options.RequestHeaders.Add("X-KEY");
    options.RequestHeaders.Add(HeaderNames.ContentEncoding);
    options.RequestHeaders.Add(HeaderNames.ContentLength);
});

// Add services to the container.
builder.Services.AddDistributedRedisCache(o =>
            {
                o.Configuration = builder.Configuration["Redis:Cn"];
            });

builder.Services.ConfigureInfrastructure(builder.Configuration);
builder.Services.ConfigureMediatr();

builder.Services.ConfigureCorsPolicy(corsPolicy);
builder.Services.ConfiguVersioningApi();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.ConfigureSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

var versionSet = app.NewApiVersionSet()
                    .HasApiVersion(version1)
                    .HasApiVersion(version2)
                    .HasApiVersion(version3)
                    .ReportApiVersions()
                    .Build();

app.UseSerilogRequestLogging();
app.UseHttpLogging();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(opts =>
{
    var descriptions = app.DescribeApiVersions();
    foreach (var desc in descriptions)
    {
        var url = $"/swagger/{desc.GroupName}/swagger.json";
        var name = desc.GroupName.ToUpperInvariant();
        opts.SwaggerEndpoint(url, $"Authenticate API {name}");
    }
});

app.UseHttpsRedirection();

//BackendRutes v1
app.UseBackendVersion1Routes(versionSet);
app.UseErrorHandler(logger);

app.Run();
