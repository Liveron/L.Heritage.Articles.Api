using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using L.Heritage.Articles.Model.ViewModel;
using MongoDB.Bson;

namespace L.Heritage.Articles.Model;

public class Article(string title, JsonElement content, ArticlePreview preview)
{
    public ObjectId Id { get; init; }

    public string Title { get; init; } = title;

    public JsonElement Content { get; init; } = content;

    [Required]
    public ArticlePreview Preview { get; init; } = preview;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

internal static class ArticleExtensions
{
    public static ArticleVM ToArticleVM(this Article article)
    {
        return new ArticleVM(
            article.Id.ToString(),
            article.Title,
            article.Content,
            article.Preview,
            article.CreatedAt);
    }
}
