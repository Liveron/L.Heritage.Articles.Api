using System.ComponentModel.DataAnnotations;

namespace L.Heritage.Articles.Model;

public class Article
{
    public int Id { get; set; }

    [Required]
    public string Tilte { get; set; } = null!;

    public string Content { get; set; } = string.Empty;
    public ArticlePreview? Preview { get; set; }
}
