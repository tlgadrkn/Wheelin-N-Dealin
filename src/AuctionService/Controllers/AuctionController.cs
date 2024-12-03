using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.Data;
using AuctionService.Dtos;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuctionController> _logger;
        public AuctionController(AuctionDbContext context, IMapper mapper, ILogger<AuctionController> logger)
        {   
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(){
        var auctions = await _context.Auctions.Include(auction => auction.Item).OrderBy(auction => auction.Item.Make).ToListAsync();
        var auctionDtos = _mapper.Map<List<AuctionDto>>(auctions);
        return Ok(auctionDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id) {
        var auction = await _context.Auctions.
        Include(auction => auction.Item)
        .FirstOrDefaultAsync(auction => auction.Id == id);
        if (auction == null) {
            _logger.LogInformation("No auction found with id: {id}", id);
            return NotFound();
        }
        return _mapper.Map<AuctionDto>(auction);
    }
    [HttpPost]
    public async Task<ActionResult<AuctionDto>> CreateAuction([FromBody] CreateAuctionDto createAuctionDto) {
        var auction = _mapper.Map<Auction>(createAuctionDto);
        // @TODO: Add current user as seller
        auction.Seller = "test";

        // ef tracking this, in memory. not saved to db.
        _context.Auctions.Add(auction);

        var result =  await _context.SaveChangesAsync() > 0;

        if (result) {
            // 201 created, CreatedAtAction returns 201 status code and useful header like location to provide the url to the created resource
            // following RESTful principles
            return CreatedAtAction(nameof(GetAuctionById), new {id = auction.Id}, _mapper.Map<AuctionDto>(auction));
        } else {
            return BadRequest("Could not create auction");
        }

    }

    [HttpPut("{id}")]
    // The client knows the updated data, so we do not need to return the updated data
    public async Task<ActionResult> UpdateAuction(Guid id, [FromBody] UpdateAuctionDto updateAuctionDto) {
        var auction = await _context.Auctions.Include(auction => auction.Item).FirstOrDefaultAsync(auction => auction.Id == id);
        if (auction == null) {
            return NotFound("No auction found with id: {id}");
        }

        auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
        auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
        auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;
        auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
        auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
        auction.UpdatedAt = DateTime.UtcNow;
        
        var success = await _context.SaveChangesAsync() > 0;
        _logger.LogInformation("Updated auction with id: {id}", id);
        if (success) {
            return Ok(_mapper.Map<AuctionDto>(auction));
        } else {
            return BadRequest("Could not update auction");
        }
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAuction(Guid id) {
        var auction =  await _context.Auctions.FindAsync(id);
        if (auction == null) {
            return NotFound("No auction found with id: {id}");
        }

        _context.Auctions.Remove(_mapper.Map<Auction>(auction));
        var success = await _context.SaveChangesAsync() > 0;
        if (success) {
            return Ok();
        } else {
            return BadRequest("Could not delete auction");
        }
    }
    };
}