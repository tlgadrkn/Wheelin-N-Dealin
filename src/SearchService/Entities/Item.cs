using System;
using MongoDB.Entities;

namespace SearchService.Entities;

// Seearch Service searches for Auction items within the Auctions microservice so it needs to know what an item is and the item is basically a class that represents the Auction entity in the AuctionsDto microservice


public class Item: Entity // Entity is a class from the MongoDB.Entities namespace, this is going to generate an Id for the item
{
    public int ReservePrice { get; set; }
    public required string Seller { get; set; }
    public string? Winner { get; set; }
    public int? SoldAmount { get; set; }
    public int? CurrentHighBid { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime AuctionEnd { get; set; }
    public required string Status { get; set; }
    public required string Make { get; set; }
    public required string Model { get; set; }
    public int Year { get; set; }
    public required string Color { get; set; }
    public int Mileage { get; set; }
    public required string ImageUrl { get; set; }
}
