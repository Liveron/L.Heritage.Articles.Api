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
            .AddServices();
        
    }
    private static IHostApplicationBuilder AddMongoDB(this IHostApplicationBuilder builder)
    {
        string? connectionString = Environment.GetEnvironmentVariable("HERITAGE_ARTICLES_DB");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Environment variable \"HERITAGE_ARTICLES_DB\" is not set");

        BsonSerializer.RegisterSerializer(new JsonElementToBsonSerializer());

        builder.Services.AddSingleton(new MongoClient(connectionString).GetDatabase("articles"));

        return builder;
    }

    private static IHostApplicationBuilder AddServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<ArticlesServices>();

        return builder;
    }
}
