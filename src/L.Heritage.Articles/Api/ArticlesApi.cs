using L.Heritage.Articles.Infrastructure;
using L.Heritage.Articles.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
using System.Threading.Tasks;

namespace L.Heritage.Articles.Api;

public static class ArticlesApi
{
    public static IEndpointRouteBuilder MapArticlesApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/articles").HasApiVersion(1.0);

        api.MapGet("/", GetAllArticles);
        api.MapGet("/{id:int}", GetArticleById);
        api.MapGet("/{articleId:int}/preview", GetArticlePreviewById);

        api.MapPut("/", UpdateArticle);
        api.MapPost("/", CreateArticle);
        api.MapDelete("/{id:int}", DeleteArticleById);

        return app;
    }

    public static async Task<Results<Ok<List<Article>>, BadRequest<string>>> GetAllArticles(
        HttpResponse response,
        [AsParameters] RequestParameters requestParameters,
        [AsParameters] ArticlesServices services)
    {
        int pageSize = requestParameters.PageSize;
        int pageNumber = requestParameters.PageNumber;

        int totalArticles = await services.Context.Articles.CountAsync();

        List<Article> articles = await services.Context.Articles
            .AsNoTracking()
            .OrderBy(article => article.Tilte)
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        int totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

        var paginationMetaData = new
        {
            TotalCount = totalArticles,
            PageSize = pageSize,
            CurrentPage = pageNumber,
            HasPrevious = pageNumber > 1,
            HasNext = pageNumber < totalPages,
        };

        response.Headers["X-Pagination"] = JsonSerializer.Serialize(paginationMetaData);

        return TypedResults.Ok(articles);
    }

    public static async Task<Results<Ok<Article>, NotFound, BadRequest<string>>> GetArticleById(
        [AsParameters] ArticlesServices services,
        int id)
    {
        if (id <= 0)
            return TypedResults.BadRequest("Id is not valid.");

        var item = await services.Context.Articles.SingleOrDefaultAsync(a => a.Id == id);

        if (item == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(item);
    }

    public static async Task<Results<Ok<ArticlePreview>, NotFound>> GetArticlePreviewById(
        ArticlesContext context, int articleId)
    {
        var preview = await context.Previews.AsNoTracking()
            .FirstOrDefaultAsync(p => p.ArticleId == articleId);

        if (preview is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(preview);
    }

    public static async Task<Results<NoContent, NotFound<string>>> UpdateArticle(
        [AsParameters] ArticlesServices services, Article articleToUpdate)
    {
        var article = await services.Context.Articles.SingleOrDefaultAsync(
            a => a.Id == articleToUpdate.Id);

        if (article is null)
            return TypedResults.NotFound($"Article with id {articleToUpdate.Id} not found.");

        EntityEntry<Article> articleEntry = services.Context.Articles.Entry(article);
        articleEntry.CurrentValues.SetValues(articleToUpdate);

        await services.Context.SaveChangesAsync();

        return TypedResults.NoContent();
    }

    public static async Task<Created> CreateArticle(
        ArticlesContext context, Article article)
    {
        await context.Articles.AddAsync(article);
        await context.SaveChangesAsync();

        return TypedResults.Created($"/api/articles/{article.Id}");
    }

    public static async Task<Results<NoContent, NotFound>> DeleteArticleById(
        ArticlesContext context, int id)
    {
        var article = context.Articles.SingleOrDefault(a => a.Id == id);

        if (article is null ) 
            return TypedResults.NotFound();

        context.Articles.Remove(article);
        await context.SaveChangesAsync();
        return TypedResults.NoContent();
    }
}
