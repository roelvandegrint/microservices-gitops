using MassTransit;
using Microservices.GitOps.MassTransit.Events;
using Public.Api.Configuration;
using Public.Api.Services;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMongoDb(builder.Configuration);

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
        cfg.Message<EmployeeUpdatedEvent>(t => t.SetEntityName("employee-updated"));
        cfg.Message<EmployeeDeletedEvent>(t => t.SetEntityName("employee-deleted"));
    });
});

var serviceName = "Microservices.GitOps.Public.API";
var serviceVersion = "0.1.0";

builder.Services.AddOpenTelemetry()
    .WithTracing(b => b
        .AddSource(serviceName)
        .SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddSqlClientInstrumentation()
        .AddOtlpExporter(o =>
        {
            o.Endpoint = new Uri(builder.Configuration.GetValue<string>("Jaeger:GrpcEndpoint"));
            o.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })
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
