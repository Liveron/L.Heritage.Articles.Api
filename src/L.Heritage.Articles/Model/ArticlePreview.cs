using L.Heritage.Articles.Model.ViewModel;
using MongoDB.Bson;

namespace L.Heritage.Articles.Model;

public class ArticlePreview(string title, string image)
{
    public string Title { get; set; } = title;

    public string Image { get; set; } = image;
}

internal static class ArticlePreviewExtensions
{
    public static ArticlePreviewVM ToArticlePreviewVM(this ArticlePreview self, string id)
    {
        return new ArticlePreviewVM(id, self.Title, self.Image);
    }
}