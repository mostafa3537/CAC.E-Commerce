namespace CAC.Application.Common.Models;

public class PagedQuery
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc"; // "asc" or "desc"
}

