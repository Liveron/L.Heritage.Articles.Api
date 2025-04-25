using L.Heritage.Articles.Infrastructure.EntityConfigurations;
using L.Heritage.Articles.Model;
using Microsoft.EntityFrameworkCore;

namespace L.Heritage.Articles.Infrastructure;

public class ArticlesContext(DbContextOptions<ArticlesContext> options) : DbContext(options)
{
    public DbSet<Article> Articles { get; set; } = null!;
    public DbSet<ArticlePreview> Previews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new ArticleEntityTypeConfiguration());
        builder.ApplyConfiguration(new ArticlePreviewEntityTypeConfiguration());
    }
}
