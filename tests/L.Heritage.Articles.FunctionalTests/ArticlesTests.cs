using L.Heritage.Articles.Infrastructure;
using L.Heritage.Articles.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning.Http;
using Asp.Versioning;

namespace L.Heritage.Articles.FunctionalTests;

public sealed class ArticlesTests :
    IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _applicationFactory;
    private readonly HttpClient _httpClient;

    public ArticlesTests(WebApplicationFactory<Program> applicationFactory)
    {
        var handler = new ApiVersionHandler(new QueryStringApiVersionWriter(), new ApiVersion(1.0));

        _applicationFactory = applicationFactory;
        _httpClient = _applicationFactory.CreateDefaultClient(handler);
    }

    [Fact]
    public async Task InsertPreviewWithSameId_ShouldThrowException()
    {
        // Arrange
        using var scope = _applicationFactory.Services.CreateScope();
        var articlesContext = scope.ServiceProvider.GetRequiredService<ArticlesContext>();

        // Act
        var article = new Article
        {
            Id = 1,
            Tilte = string.Empty,
            Preview = new ArticlePreview { Title = string.Empty }
        };

        var preview = new ArticlePreview { ArticleId = 1 };

        // Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await articlesContext.AddAsync(article);
            await articlesContext.AddAsync(preview);
            await articlesContext.SaveChangesAsync();
        });
    }
}
