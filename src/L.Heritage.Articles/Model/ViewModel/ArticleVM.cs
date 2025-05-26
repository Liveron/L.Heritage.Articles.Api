using System.Text.Json;

namespace L.Heritage.Articles.Model.ViewModel;

public record ArticleVM(
    string Id, 
    string Title, 
    JsonElement Content,
    ArticlePreview Preview);