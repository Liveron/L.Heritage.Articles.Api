using L.Heritage.Articles.Model;
using L.Heritage.Articles.Model.DTOs;
using L.Heritage.Articles.Model.ViewModel;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using System.Text.Json;

namespace L.Heritage.Articles.Api;

public static class ArticlesApi
{
    public static IEndpointRouteBuilder MapArticlesApiV1(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("api/articles").HasApiVersion(1.0);

        api.MapGet("/", GetAllArticles);
        //api.MapGet("/{id:int}", GetArticleById);
        //api.MapGet("/{articleId:int}/preview", GetArticlePreviewById);

        //api.MapPut("/", UpdateArticle);
        api.MapPost("/", CreateArticle);
        //api.MapDelete("/{id:int}", DeleteArticleById);

        return app;
    }

    public static async Task<Results<Ok<PaginatedItems<ArticleVM>>, BadRequest<string>>> GetAllArticles(
        [AsParameters] ArticlesServices services,
        [AsParameters] RequestParameters requestParameters)
    {
        var pageSize = requestParameters.PageSize;
        var pageNumber = requestParameters.PageNumber;

        var articlesCollection = GetArticlesCollection(services.Database);

        var totalArticles = await articlesCollection.CountDocumentsAsync("{}");

        var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

        var articles = await articlesCollection.Find("{}")
            .Skip(pageSize * (pageNumber - 1))
            .Limit(pageSize)
            .ToListAsync();

        var articleVms = articles.ConvertAll(a => a.ToArticleVM());

        return TypedResults.Ok(new PaginatedItems<ArticleVM>(
            pageNumber: pageNumber,
            totalCount: totalArticles,
            hasPrevious: pageNumber > 1,
            hasNext: pageNumber < totalPages,
            data: articleVms));
    }

    //public static async Task<Results<Ok<Article>, NotFound, BadRequest<string>>> GetArticleById(
    //    [AsParameters] ArticlesServices services,
    //    int id)
    //{
    //    if (id <= 0)
    //        return TypedResults.BadRequest("Id is not valid.");

    //    var item = await services.Context.Articles.SingleOrDefaultAsync(a => a.Id == id);

    //    if (item == null)
    //        return TypedResults.NotFound();

    //    return TypedResults.Ok(item);
    //}

    //public static async Task<Results<Ok<ArticlePreview>, NotFound>> GetArticlePreviewById(
    //    ArticlesContext context, int articleId)
    //{
    //    var preview = await context.Previews.AsNoTracking()
    //        .FirstOrDefaultAsync(p => p.ArticleId == articleId);

    //    if (preview is null)
    //        return TypedResults.NotFound();

    //    return TypedResults.Ok(preview);
    //}

    //public static async Task<Results<NoContent, NotFound<string>>> UpdateArticle(
    //    [AsParameters] ArticlesServices services, Article articleToUpdate)
    //{
    //    var article = await services.Context.Articles.SingleOrDefaultAsync(
    //        a => a.Id == articleToUpdate.Id);

    //    if (article is null)
    //        return TypedResults.NotFound($"Article with id {articleToUpdate.Id} not found.");

    //    EntityEntry<Article> articleEntry = services.Context.Articles.Entry(article);
    //    articleEntry.CurrentValues.SetValues(articleToUpdate);

    //    await services.Context.SaveChangesAsync();

    //    return TypedResults.NoContent();
    //}

    public static async Task<Created> CreateArticle(
        IMongoDatabase database, CreateArticleDTO articleDto, [FromServices] ArticlesServices services)
    {
        services.Logger.LogInformation($"Method api/articles CreateArticle started. " +
            $"Request: {JsonSerializer.Serialize(articleDto)}");

        var articlesCollection = GetArticlesCollection(database);

        var article = new Article(articleDto.Title, articleDto.Content, articleDto.Preview);

        await articlesCollection.InsertOneAsync(article);

        return TypedResults.Created($"/api/articles/{article.Id}");
    }

    //public static async Task<Results<NoContent, NotFound>> DeleteArticleById(
    //    ArticlesContext context, int id)
    //{
    //    var article = context.Articles.SingleOrDefault(a => a.Id == id);

    //    if (article is null ) 
    //        return TypedResults.NotFound();

    //    context.Articles.Remove(article);
    //    await context.SaveChangesAsync();
    //    return TypedResults.NoContent();
    //}

    private static IMongoCollection<Article> GetArticlesCollection(IMongoDatabase database) =>
        database.GetCollection<Article>("articles");

}
