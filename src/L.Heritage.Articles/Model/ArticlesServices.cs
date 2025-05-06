using MongoDB.Driver;

namespace L.Heritage.Articles.Model;

public class ArticlesServices(
    ILogger<ArticlesServices> logger,
    IMongoDatabase database)
{
    public IMongoDatabase Database { get; } = database;
    public ILogger<ArticlesServices> Logger { get; } = logger;
}
