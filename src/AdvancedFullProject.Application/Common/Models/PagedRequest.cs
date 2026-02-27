namespace AdvancedFullProject.Application.Common.Models;

public sealed class PagedRequest
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Filter { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool Descending { get; set; } = true;
}
