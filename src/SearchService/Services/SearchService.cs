using MongoDB.Entities;
using SearchService.Dtos;
using SearchService.Entities;
using SearchService.RequestHelpers;

namespace SearchService.Services
{
    public class SearchService : ISearchService
    {

        public SearchService()
        {

        }

        public async Task<SearchResponseDto<Item>> SearchAsync(SearchParameters searchParameters)
        {
            ValidateSearchParameters(searchParameters);

            var query = DB.PagedSearch<Item>().Sort(item => item.Descending(i => i.Make));

            if (!string.IsNullOrEmpty(searchParameters.SearchTerm))
            {
                query = query.Match(Search.Full, searchParameters.SearchTerm).SortByTextScore();
            }

            query = searchParameters.OrderBy switch
            {
                "make" => query.Sort(x => x.Ascending(x => x.Make)),
                "new" => query.Sort(x => x.Descending(x => x.CreatedAt)),
                _ => query.Sort(x => x.Ascending(x => x.AuctionEnd))
            };

            if (!string.IsNullOrEmpty(searchParameters.FilterBy))
            {
                query = searchParameters.FilterBy switch
                {
                    "finished" => query.Match(x => x.AuctionEnd < DateTime.UtcNow),
                    "endingSoon" => query.Match(x =>
                        x.AuctionEnd < DateTime.UtcNow.AddHours(6) 
                            && x.AuctionEnd > DateTime.UtcNow),
                    _ => query.Match(x => x.AuctionEnd > DateTime.UtcNow) // live
                };
            }

            if (!string.IsNullOrEmpty(searchParameters.Seller))
            {
                query.Match(x => x.Seller == searchParameters.Seller);
            }

            if (!string.IsNullOrEmpty(searchParameters.Winner))
            {
                query.Match(x => x.Winner == searchParameters.Winner);
            }

            query.PageNumber(searchParameters.PageNumber);
            query.PageSize(searchParameters.PageSize);

            var result = await query.ExecuteAsync();

            return new SearchResponseDto<Item>
            {
                Results = result.Results.ToList(),
                PageCount = result.PageCount,
                TotalCount = result.TotalCount
            };
        }

        private void ValidateSearchParameters(SearchParameters parameters)
        {
            if (parameters == null)
                throw new ArgumentNullException(nameof(parameters));
            
            if (parameters.PageNumber < 1)
                throw new ArgumentException("Page number must be greater than 0");
            
            if (parameters.PageSize < 1)
                throw new ArgumentException("Page size must be greater than 0");
        }

    }
}