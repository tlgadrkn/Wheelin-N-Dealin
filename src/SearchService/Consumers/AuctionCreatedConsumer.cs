using AutoMapper;
using MassTransit;
using MongoDB.Entities;
using Contracts;
using SearchService.Entities;

namespace SearchService.Consumers;

public class AuctionCreatedConsumer(IMapper mapper) : IConsumer<AuctionCreated>
{
    public async Task Consume(ConsumeContext<AuctionCreated> context)
    {
        Console.WriteLine("--> Consuming Auction Created: " + context.Message.Id);

        var item = mapper.Map<Item>(context.Message);
        // This is a test to see if the message is faulty and if it is, we will throw an exception and it will go to the AuctionCreatedFaultConsumer. We do this to test to avoid losing any messages.
        if (item.Model == "Foo") throw new ArgumentException("Cannot sell cars with name of Foo");

        await item.SaveAsync();
    }
}
