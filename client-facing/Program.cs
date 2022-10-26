using MultiArchApi.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IHostnameService, HostNameService>();
var baseUri = builder.Configuration.GetValue<string>("BackendApiBaseAddress");
builder.Services.AddHttpClient<HostNameService>(options =>
{
    options.BaseAddress = new Uri(baseUri);
});
builder.Services.AddHttpClient<HostNameController>(options =>
{
    options.BaseAddress = new Uri(baseUri);
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
