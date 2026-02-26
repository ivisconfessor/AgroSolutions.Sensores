using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace AgroSolutions.Sensores.Infra;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSensoresInfra(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoConnection = configuration["MongoDb:ConnectionString"]
            ?? throw new InvalidOperationException("MongoDb:ConnectionString não está configurada.");
        var databaseName = configuration["MongoDb:DatabaseName"] ?? "agrosolutions_sensores";

        var mongoClient = new MongoClient(mongoConnection);
        var database = mongoClient.GetDatabase(databaseName);
        services.AddSingleton<IMongoDatabase>(database);
        services.AddSingleton<ILeituraSensorRepository, LeituraSensorRepository>();

        var rabbitHost = configuration["RabbitMQ:HostName"] ?? "localhost";
        var rabbitUser = configuration["RabbitMQ:UserName"];
        var rabbitPassword = configuration["RabbitMQ:Password"];
        var filaLeituras = configuration["RabbitMQ:FilaLeituras"] ?? "agrosolutions.sensores.leituras";
        services.AddSingleton<IRabbitMqPublicador>(sp => new RabbitMqPublicador(rabbitHost, rabbitUser, rabbitPassword, filaLeituras));

        return services;
    }
}
