using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace L.Heritage.Articles.Model;

public class Article
{
    [BsonId]
    public int Id { get; set; }

    [Required]
    public string Tilte { get; set; } = null!;

    public JsonElement Content { get; set; }
    //public ArticlePreview? Preview { get; set; }
}
