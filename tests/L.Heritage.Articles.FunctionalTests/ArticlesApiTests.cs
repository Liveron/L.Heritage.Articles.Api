using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using L.Heritage.Articles.Model;
using L.Heritage.Articles.Model.ViewModel;
using Helpers;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using L.Heritage.Articles.Model.DTOs;
using System.Text.Json;

namespace L.Heritage.Articles.FunctionalTests;

public sealed class ArticlesApiTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _applicationFactory;
    private readonly HttpClient _httpClient;

    public ArticlesApiTests(WebApplicationFactory<Program> applicationFactory)
    {
        //var handler = new ApiVersionHandler(new QueryStringApiVersionWriter(), new ApiVersion(1.0));

        _applicationFactory = applicationFactory;

        _httpClient = _applicationFactory.CreateDefaultClient();
    }

    [Fact]
    public async Task GetAllArticlesRespectsPageSize()
    {
        // Act
        ReinitializeDb();
        var response = await _httpClient.GetAsync("/api/articles?PageNumber=1&PageSize=3");

        // Arrange
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedItems<ArticleVM>>();

        // Assert
        Assert.Equal(5, result!.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(3, result.Data.Count());
    }

    [Fact]
    public async Task GetAllArticlePreviewsRespectsPageSize()
    {
        ReinitializeDb();
        var response = await _httpClient.GetAsync("/api/articles/previews?PageNumber=1&PageSize=3");

        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedItems<ArticlePreviewVM>>();

        Assert.Equal(5, result!.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(3, result.Data.Count());
    }

    [Fact]
    public async Task GetArticleById()
    {
        // Act
        ReinitializeDb();
        var response = await _httpClient.GetAsync("/api/articles?PageNumber=1&PageSize=1");
        var result = await response.Content.ReadFromJsonAsync<PaginatedItems<ArticleVM>>();
        var article = result!.Data.FirstOrDefault();

        var id = article!.Id;
        var secondReponse = await _httpClient.GetAsync($"/api/articles/{id}");
        var secondResult = await secondReponse.Content.ReadFromJsonAsync<ArticleVM>();

        Assert.NotNull(secondResult);
        Assert.Equal(secondResult.Id, id);
    }


    [Fact]
    public async Task AddArticle()
    {
        // Act - 1
        ReinitializeDb();
        var content = JsonSerializer.SerializeToElement(new { Data = "Some Data" });
        var preview = new ArticlePreview("New Article", "Some image link");
        var bodyContent = new CreateArticleDTO("New Article", content, preview);
        var response = await _httpClient.PostAsJsonAsync("/api/articles", bodyContent);
        response.EnsureSuccessStatusCode();

        // Act - 2
        response = await _httpClient.GetAsync("/api/articles?PageNumber=1&PageSize=6");
        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<PaginatedItems<ArticleVM>>();
        var articles = items!.Data;

        // Assert
        Assert.Contains(articles, a => a.Title == "New Article");
    }

    [Fact]
    public async Task DeleteArticle()
    {
        // Act - 1
        ReinitializeDb();
        var response = await _httpClient.GetAsync("/api/articles?PageNumber=1&PageSize=1");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<PaginatedItems<ArticleVM>>();
        var article = result!.Data.FirstOrDefault();

        var id = article!.Id;
        var secondResponse = await _httpClient.DeleteAsync($"/api/articles/{id}");
        secondResponse.EnsureSuccessStatusCode();

        var thirdResponse = await _httpClient.GetAsync($"/api/articles/{id}");
        var responseStatus = thirdResponse.StatusCode;

        // Assert
        Assert.Equal("NoContent", secondResponse.StatusCode.ToString());
        Assert.Equal("NotFound", responseStatus.ToString());
    }

    private void ReinitializeDb()
    {
        var mongoDb = _applicationFactory.Services.GetRequiredService<IMongoDatabase>();
        Utilities.ReinitializeDbForTests(mongoDb);
    }
}
