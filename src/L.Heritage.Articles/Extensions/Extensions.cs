using L.Heritage.Articles.Infrastructure.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace L.Heritage.Articles.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.AddMongoDB();
    }
    private static void AddMongoDB(this IHostApplicationBuilder builder)
    {
        string? connectionString = Environment.GetEnvironmentVariable("HERITAGE_ARTICLES_DB");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Environment variable \"HERITAGE_ARTICLES_DB\" is not set");

        BsonSerializer.RegisterSerializer(new JsonElementToBsonSerializer());

        builder.Services.AddSingleton(new MongoClient(connectionString).GetDatabase("articles"));
    }
}
