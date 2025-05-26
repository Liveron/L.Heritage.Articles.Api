using L.Heritage.Articles.Infrastructure.Serializers;
using L.Heritage.Articles.Model;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace L.Heritage.Articles.Extensions;

internal static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder
            .AddMongoDB()
            .AddServices()
            .AddEnvironment();
    }

    private static IHostApplicationBuilder AddEnvironment(this IHostApplicationBuilder builder)
    {
        builder.Configuration.AddEnvironmentVariables(prefix: "ARTICLES_");
        return builder;
    }

    private static IHostApplicationBuilder AddMongoDB(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetValue<string>("ARTICLES_DB");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Environment variable \"ARTICLES_DB\" is not set");

        BsonSerializer.TryRegisterSerializer(new JsonElementToBsonSerializer());

        builder.Services.AddSingleton(new MongoClient(connectionString).GetDatabase("articles"));

        return builder;
    }

    private static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ArticlesServices>();

        return builder;
    }
}
