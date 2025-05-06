using System.ComponentModel.DataAnnotations;

namespace L.Heritage.Articles.Model;

public class ArticlePreview
{
    [Required]
    public string Title { get; set; } = null!;

    public string Image { get; set; } = string.Empty;
}
