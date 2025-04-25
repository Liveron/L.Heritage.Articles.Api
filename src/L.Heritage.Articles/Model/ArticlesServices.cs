using L.Heritage.Articles.Infrastructure;

namespace L.Heritage.Articles.Model;

public class ArticlesServices(ArticlesContext context)
{
    public ArticlesContext Context { get; init; } = context;
}
