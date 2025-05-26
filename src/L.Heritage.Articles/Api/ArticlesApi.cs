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
        api.MapGet("/{id}", GetArticleById);
        api.MapGet("/previews", GetAllArticlePreviews);

        //api.MapPut("/", UpdateArticle);
        api.MapPost("/", CreateArticle);
        api.MapDelete("/{id}", DeleteArticleById);

        return app;
    }

    public static async Task<Results<Ok<PaginatedItems<ArticleVM>>, BadRequest<string>>> GetAllArticles(
        [AsParameters] ArticlesServices services,
        [AsParameters] RequestParameters requestParameters)
    {
        var pageSize = requestParameters.PageSize;
        var pageNumber = requestParameters.PageNumber;

        var articlesCollection = GetArticlesCollection(services.Database);

        var emptyFilter = FilterDefinition<Article>.Empty;

        var totalArticles = await articlesCollection.CountDocumentsAsync(emptyFilter);

        var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

        var articles = await articlesCollection.Find(emptyFilter)
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

    public static async Task<Results<Ok<ArticleVM>, NotFound, BadRequest<string>>> GetArticleById(
        [AsParameters] ArticlesServices services,
        string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return TypedResults.BadRequest("Id is not valid.");

        var articlesCollection = GetArticlesCollection(services.Database);

        var filter = Builders<Article>.Filter.Eq(a => a.Id, id);

        var article = await articlesCollection.Find(filter).FirstOrDefaultAsync();

        if (article is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(article.ToArticleVM());
    }

    public static async Task<Ok<PaginatedItems<ArticlePreviewVM>>> GetAllArticlePreviews(
        [AsParameters] ArticlesServices services,
        [AsParameters] RequestParameters requestParameters)
    {
        var pageSize = requestParameters.PageSize;
        var pageNumber = requestParameters.PageNumber;

        var articlesCollection = GetArticlesCollection(services.Database);

        var emptyFilter = FilterDefinition<Article>.Empty;

        var totalArticles = await articlesCollection.CountDocumentsAsync(emptyFilter);

        var totalPages = (int)Math.Ceiling(totalArticles / (double)pageSize);

        var articles = await articlesCollection.Find(emptyFilter)
            .Skip(pageSize * (pageNumber - 1))
            .Limit(pageSize)
            .ToListAsync();

        var previewVMs = articles.ConvertAll(a => a.Preview.ToArticlePreviewVM(a.Id));

        return TypedResults.Ok(new PaginatedItems<ArticlePreviewVM>(
            pageNumber: pageNumber,
            totalCount: totalArticles,
            hasPrevious: pageNumber > 1,
            hasNext: pageNumber < totalPages,
            data: previewVMs));
    }

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
        CreateArticleDTO articleDto, 
        [FromServices] ArticlesServices services)
    {
        services.Logger.LogInformation($"Method api/articles CreateArticle started. " +
            $"Request: {JsonSerializer.Serialize(articleDto)}");

        var articlesCollection = GetArticlesCollection(services.Database);

        var article = new Article(articleDto.Title, articleDto.Content, articleDto.Preview);

        await articlesCollection.InsertOneAsync(article);

        return TypedResults.Created($"/api/articles/{article.Id}");
    }

    public static async Task<Results<NoContent, NotFound>> DeleteArticleById(
        [FromServices] ArticlesServices services,
        string id)
    {
        var articlesCollection = GetArticlesCollection(services.Database);

        var filter = Builders<Article>.Filter.Eq(a => a.Id, id);

        var result = await articlesCollection.DeleteOneAsync(filter);

        if (result.DeletedCount < 1)
            return TypedResults.NotFound();

        return TypedResults.NoContent();
    }

    private static IMongoCollection<Article> GetArticlesCollection(IMongoDatabase database) =>
        database.GetCollection<Article>("articles");
}
