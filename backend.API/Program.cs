using Backend.Api.Configuration;
using Backend.Api.Events.Consumers;
using Backend.API.Persistence;
using MassTransit;
using Microservices.GitOps.MassTransit.Events;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<DatabaseOptions>().Bind(builder.Configuration.GetSection(nameof(DatabaseOptions)));
builder.Services.AddDbContext<EmployeeDbContext>();

var massTransitOptions = builder.Configuration.GetSection(nameof(MassTransitOptions)).Get<MassTransitOptions>();
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmployeeCreatedEventConsumer>();
    x.AddConsumer<EmployeeDeletedEventConsumer>();

    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(massTransitOptions.AzureServiceBusConnectionString);

        cfg.Message<EmployeeCreatedEvent>(x => x.SetEntityName("employee-created"));
        cfg.Message<EmployeeDeletedEvent>(x => x.SetEntityName("employee-deleted"));

        cfg.SubscriptionEndpoint<EmployeeCreatedEvent>("backend-api-employee-created", e =>
            e.ConfigureConsumer<EmployeeCreatedEventConsumer>(context));

        cfg.SubscriptionEndpoint<EmployeeDeletedEvent>("backend-api-employee-deleted", e =>
            e.ConfigureConsumer<EmployeeDeletedEventConsumer>(context));

        cfg.ConfigureEndpoints(context);
    });
});

var serviceName = "Microservices.GitOps.Backend.API";
var serviceVersion = "0.1.0";

builder.Services.AddOpenTelemetry()
    .WithTracing(builder => builder
        .AddConsoleExporter()
        .AddSource(serviceName)
        .AddOtlpExporter()
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
