using System;
using SearchService.Dtos;
using SearchService.Entities;
using SearchService.RequestHelpers;

namespace SearchService.Services;

public interface ISearchService
{
    Task<SearchResponseDto<Item>> SearchAsync(SearchParameters searchParameters);
}
