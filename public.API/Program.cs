using MassTransit;
using Microservices.GitOps.MassTransit.Events;
using Public.Api.Configuration;
using Public.API.Persistence;

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
    x.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(massTransitOptions.AzureServiceBusConnectionString);

        cfg.Message<EmployeeCreatedEvent>(t => t.SetEntityName("employee-created-new"));
    });
});

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
