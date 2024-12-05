using Microsoft.AspNetCore.Mvc;
using MongoDB.Entities;
using SearchService.Entities;
using SearchService.RequestHelpers;
using SearchService.Services;

namespace SearchService.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;
    private readonly ILogger<SearchController> _logger;

    public SearchController(ISearchService searchService, ILogger<SearchController> logger)
     {
            _searchService = searchService;
            _logger = logger;
     }

    [HttpGet]
    public async Task<ActionResult<List<Item>>> SearchItems([FromQuery] SearchParameters searchParameters) {
        
        try {
            var result = await _searchService.SearchAsync(searchParameters);
            if (result.Results == null || !result.Results.Any())
            {
                 _logger.LogInformation("No items found for the given search parameters.");
                 return NotFound();
            } 

            return Ok(result);
        } catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while searching items");
            return StatusCode(500, "An error occurred while searching items");
        }

    }
    
}
