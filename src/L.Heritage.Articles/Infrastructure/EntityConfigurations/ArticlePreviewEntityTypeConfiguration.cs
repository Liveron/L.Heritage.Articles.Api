using L.Heritage.Articles.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace L.Heritage.Articles.Infrastructure.EntityConfigurations;

public class ArticlePreviewEntityTypeConfiguration : IEntityTypeConfiguration<ArticlePreview>
{
    public void Configure(EntityTypeBuilder<ArticlePreview> builder)
    {
        builder.ToTable("Preview");

        builder.Property(ap => ap.Title)
            .HasMaxLength(50);

        builder.HasKey(preview => preview.ArticleId);
    }
}
