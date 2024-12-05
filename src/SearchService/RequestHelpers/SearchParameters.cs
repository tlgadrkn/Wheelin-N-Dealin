using System;

namespace SearchService.RequestHelpers;

public class SearchParameters
{
    public string SearchTerm { get; set; }
    public string Seller { get; set; }

    public string Winner { get; set; }
    public string OrderBy { get; set; }
    public string FilterBy { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 5;
}
