using System.Text.Json;

namespace L.Heritage.Articles.Model.DTOs;

public record CreateArticleDTO(
    string Title,
    JsonElement Content,
    ArticlePreview Preview);
