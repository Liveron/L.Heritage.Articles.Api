using System.ComponentModel;

namespace L.Heritage.Articles.Model;

public class RequestParameters
{
    const int MAX_PAGE_SIZE = 50;
    private int _pageSize = 3;
    public int PageNumber { get; init; } = 1;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MAX_PAGE_SIZE ? MAX_PAGE_SIZE : value;
    }
}
