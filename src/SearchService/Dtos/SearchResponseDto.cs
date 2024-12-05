using System;

namespace SearchService.Dtos;

public class SearchResponseDto<T>
{
    public List<T> Results { get; set; }
    public int PageCount { get; set; }
    public long TotalCount { get; set; }

}
