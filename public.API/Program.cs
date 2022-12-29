using MassTransit;
using Microservices.GitOps.MassTransit.Events;
using OpenTelemetry;
using Public.Api.Configuration;
using Public.API.Persistence;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Public.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<DatabaseOptions>().Bind(builder.Configuration.GetSection(nameof(DatabaseOptions)));
builder.Services.AddDbContext<EmployeeDbContext>();

builder.Services.AddScoped<IBackendClient, BackendClient>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpClient<IBackendClient, BackendClient>(client =>
    {
        client.BaseAddress = builder.Configuration.GetServiceUri("backend-api");
    });
}
else
{
    builder.Services.AddHttpClient<IBackendClient, BackendClient>(client =>
    {
        client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("BackendApiBaseAddress"));
    });
}

var massTransitOptions = builder.Configuration.GetSection(nameof(MassTransitOptions)).Get<MassTransitOptions>();
builder.Services.AddMassTransit(x =>
{
    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(massTransitOptions.AzureServiceBusConnectionString);

        cfg.Message<EmployeeCreatedEvent>(t => t.SetEntityName("employee-created"));
        cfg.Message<EmployeeDeletedEvent>(t => t.SetEntityName("employee-deleted"));
    });
});

var serviceName = "Microservices.GitOps.Public.API";
var serviceVersion = "0.1.0";

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
        .AddConsoleExporter()
        .AddSource(serviceName)
        .AddOtlpExporter(o => {
            o.Endpoint = new Uri(builder.Configuration.GetValue<string>("Jaeger:GrpcEndpoint"));
            o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation()
    )
    .StartWithHost();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
