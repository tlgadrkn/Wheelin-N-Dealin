using System;
using AuctionService.Dtos;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Contracts;

namespace AuctionService.RequestHelpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Auction, AuctionDto>()
        .IncludeMembers(x => x.Item);
        CreateMap<Item, AuctionDto>();
        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Item>();
        CreateMap<Auction, AuctionCreated>().IncludeMembers(a => a.Item);
        CreateMap<Auction, AuctionUpdated>().IncludeMembers(a => a.Item);
        CreateMap<Auction, AuctionDeleted>();
        CreateMap<Item, AuctionCreated>();
        CreateMap<Item, AuctionUpdated>();



    }

}
