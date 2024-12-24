using System;
using Contracts;
using MassTransit;

namespace AuctionService.Consumers;
// This is a consumer that will consume the fault message and the goal is to try to re-publish the message to the queue as we do not want to lose any messages. As a result the services that have missed the message will receive it again. This is covering the case when event bus does it's job and send the message to the service but after that there is an exception while saving to db, we cannot lose the message.
public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine("--> Consuming faulty upsert");

        var exception = context.Message.Exceptions.First();

        if (exception.ExceptionType == "System.ArgumentException")
        {
            // if the message is faulty, we will try to re-publish the message to the queue as we do not want to lose any messages.
            // changing faulty message to a valid message, so the message will be published to the queue again.
            // This specific is just for geting the idea, in real life we would have a different approach to handle the fault message. Probably just delete it from the db and do not try to re-publish it.
            // just an example...
            context.Message.Message.Model = "FooBar";
            await context.Publish(context.Message.Message);
        }
        else
        {
            Console.WriteLine("Not an argument exception - update error dashboard somewhere");
        }
    }
}
