using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using L.Heritage.Articles.Model.ViewModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L.Heritage.Articles.Model;

public class Article(string title, JsonElement content, ArticlePreview preview)
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = null!;

    public string Title { get; init; } = title;

    public JsonElement Content { get; init; } = content;

    public ArticlePreview Preview { get; init; } = preview;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

internal static class ArticleExtensions
{
    public static ArticleVM ToArticleVM(this Article self)
    {
        return new ArticleVM(
            self.Id.ToString(),
            self.Title,
            self.Content,
            self.Preview);
    }
}
