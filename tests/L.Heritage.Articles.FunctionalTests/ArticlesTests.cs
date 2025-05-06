using L.Heritage.Articles.Infrastructure;
using L.Heritage.Articles.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Asp.Versioning.Http;
using Asp.Versioning;
using Microsoft.Extensions.Configuration;

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

        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (string.IsNullOrWhiteSpace(environment) || environment != "Testing")
            DotNetEnv.Env.Load("../../.env");

        _httpClient = _applicationFactory.CreateDefaultClient(handler);
    }

    [Fact]
    public void Test()
    { }
}
