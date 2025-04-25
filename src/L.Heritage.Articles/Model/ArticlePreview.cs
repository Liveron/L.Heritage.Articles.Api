using System.ComponentModel.DataAnnotations;

namespace L.Heritage.Articles.Model;

public class ArticlePreview
{
    public int ArticleId { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    public string Image { get; set; } = string.Empty;
}
