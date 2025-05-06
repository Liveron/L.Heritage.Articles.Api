using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace L.Heritage.Articles.Model.ViewModel;

public record ArticleVM(
    string Id, 
    [Required] string Title, 
    JsonElement Content,
    ArticlePreview Preview,
    DateTime CreatedAt);