using L.Heritage.Articles.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L.Heritage.Articles.Infrastructure.EntityConfigurations;

public class ArticleEntityTypeConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.Property(a => a.Tilte)
            .HasMaxLength(50);

        builder.HasOne(a => a.Preview)
            .WithOne()
            .HasForeignKey<ArticlePreview>(ap => ap.ArticleId);
    }
}
