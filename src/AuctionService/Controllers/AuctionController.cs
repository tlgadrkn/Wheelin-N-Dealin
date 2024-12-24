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
using MassTransit;
using Contracts;
using AutoMapper.QueryableExtensions;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/auctions")]
    public class AuctionController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<AuctionController> _logger;
        private readonly IPublishEndpoint _publishEndpoint;
        public AuctionController(AuctionDbContext context, IMapper mapper, ILogger<AuctionController> logger, IPublishEndpoint publishEndpoint)
        {   
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }
        

    [HttpGet]
    public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions(string? date)
{
        var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

      if (!string.IsNullOrEmpty(date))
        {
            query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);
        }

        return await query.ProjectTo<AuctionDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
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

        // So now we're publishing the auction created event before saving to the database.
        // This will behave like a transaction, because we've added the outbox pattern. 
        // so if one fails, all fail, like a transaction.
        var newAuction = _mapper.Map<AuctionCreated>(auction);
        await _publishEndpoint.Publish(newAuction);
        // Then save (this will commit both the auction and the outbox message in a single transaction)
        var result =  await _context.SaveChangesAsync() > 0;

        if (!result) {
            // 201 created, CreatedAtAction returns 201 status code and useful header like location to provide the url to the created resource
            // following RESTful principles
            return BadRequest("Could not create auction");
        }
        return CreatedAtAction(nameof(GetAuctionById), new {id = auction.Id}, newAuction);

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
            await _publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));
            return Ok(_mapper.Map<AuctionUpdated>(auction));
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
            await _publishEndpoint.Publish(_mapper.Map<AuctionDeleted>(auction));
            return Ok();
        } else {
            return BadRequest("Could not delete auction");
        }
    }
    };
}