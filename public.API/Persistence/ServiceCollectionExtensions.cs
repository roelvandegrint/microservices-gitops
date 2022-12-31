using MongoDB.Driver;
using Public.API.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class MongoDbServiceCollectionExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configurationRoot)
    {
        var mongoDbOptionsSection = configurationRoot.GetSection(nameof(MongoDbOptions));
        services.AddOptions<MongoDbOptions>().Bind(mongoDbOptionsSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        var mongoDbOptions = mongoDbOptionsSection.Get<MongoDbOptions>();

        services.AddSingleton<IMongoClient>(serviceProvider => new MongoClient(mongoDbOptions.ConnectionString));
        services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            var mongoClient = serviceProvider.GetRequiredService<IMongoClient>();
            return mongoClient.GetDatabase(mongoDbOptions.EmployeesDatabase);
        });

        return services;
    }
}