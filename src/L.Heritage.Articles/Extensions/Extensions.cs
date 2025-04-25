using L.Heritage.Articles.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace L.Heritage.Articles.Extensions;

public static class Extensions
{
    public static void AddApplicationServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ArticlesContext>(options =>
        {
            string? connectionString = Environment.GetEnvironmentVariable("HERITAGE_ARTICLES_DB");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Environment variable \"HERITAGE_ARTICLES_DB\" is not set");

            options.UseNpgsql(connectionString);
        });
    }
}
