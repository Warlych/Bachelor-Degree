namespace Apis.Gateway.Models.Paginations;

public sealed class Pagination
{
    private const int MaximumPageSize = 100;

    private int _pageNumber;
    private int _pageSize;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1 || value > MaximumPageSize ? 20 : value;
    }

    public Pagination()
    {
        PageNumber = 1;
        PageSize = 20;
    }

    public Pagination(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}
