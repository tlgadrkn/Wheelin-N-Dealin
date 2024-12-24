# What Could Go Wrong?

When we design our microservices, we need to think about what could go wrong, and how we can handle those situations.

In this application, so far we have AuctionService and SearchService. AuctionService is responsible for creating auctions, and SearchService is responsible for searching auctions.

So what could go wrong?

Let's think about some scenarios:

In a monolith application let's say a POST request hits the Auction endpoint, and let's say Auction service has a `Transaction` that creates an auction in Auction table and then updates the search table. Once the Savechanges is called the database is updated. But what if one of these actions fails? Everthing in this transaction will fail and will be rolled back. **So there is no way that data will be inconsistent because the transaction is atomic.** Becasue in a monolith application, we use `ACID` transactions.

In database terms, a `transaction` is a `sequence of one or more database operations (such as inserts, updates, or deletes) that are treated as a single unit of work`. The primary purpose of a transaction is to ensure data integrity and consistency in a database system, especially when multiple operations need to be performed together

ACID stands for:

- **Atomicity**: All operations in a transaction are treated as a single unit of work, which either succeeds completely or fails completely.
- **Consistency**: Data is consistent before and after the transaction. A transaction must bring the database from one valid state to another valid state, maintaining all defined rules and constraints.
- **Isolation**: Transactions are isolated, Concurrent transactions should not interfere with each other. The effects of an incomplete transaction should not be visible to other transactions.
- **Durability**: Once a transaction is committed, the changes are permanent. Once a transaction is committed, its effects must persist in the database, even in the event of system failures.

Examples of transactions:

Transferring money between two bank accounts: This involves deducting money from one account and adding it to another. Both operations must succeed or fail together.

Booking a flight ticket: This might involve updating seat availability, creating a reservation record, and processing payment. All these operations should be treated as a single unit.

Updating inventory after a sale: This could involve reducing the quantity of items sold and creating an order record.

Transactions are crucial for maintaining data integrity in multi-user environments and in systems where data consistency is critical. They help prevent data corruption that could occur if only part of a series of related changes is completed.

In a microservices architecture, we don't have transactions that span multiple services. So we need to think about what could go wrong and how we can handle those situations.

So what could go wrong in our application?

So thinking about the same POST request that hits the auction service in a microservices app.

1. A POST request hits the AuctionService endpoint to create an auction.
2. AuctionService creates an auction in the database.
3. After that AuctionService is going to publish a message to the Event Bus to update the SearchService.
4. SearchService is going to consume that message.
5. SearchService is going to update the search table.

BUT what if one of these actions fails?

- What if the message is not published to the Event Bus?
- What if the message is published to the Event Bus but not consumed by the SearchService?
- What if the SearchService is down?
- What if the SearchService is up but the database is down?
- What if the SearchService is up but the message is lost?
- What if the SearchService is up but the message is corrupted?

So many things can go wrong in a microservices architecture.

So we need to think about how we can handle these situations.

So we need to think about how we can make our microservices resilient, fault-tolerant, and highly available.

If one of our service fails, and a user attempts to create an auction whilst that service is down, will the AuctionService and SearchService be consistent?

Let's think about some scenarios:

1. AuctionService is down, AuctionServiceDB is up, SearchService is up, SearchServiceDB is up, EventBus is up. = A user attempts to create an auction, AuctionService is down, so the auction is not created so it's never going to trigger the message to the Event Bus, so the SearchService is not going to be updated. So the AuctionService and SearchService are consistent.
2. AuctionService is up, AuctionServiceDB is down, SearchService is up, SearchServiceDB is up, EventBus is up. = A user attempts to create an auction, AuctionService is up, but the database is down, so the auction is not created so it's never going to trigger the message to the Event Bus, so the SearchService is not going to be updated. So the AuctionService and SearchService are consistent.
3. AuctionService is up, AuctionServiceDB is up, SearchService is down, SearchServiceDB is up, EventBus is up. = A user attempts to create an auction, AuctionService is up, the auction is created, the message is published to the Event Bus, but the SearchService is down, so the SearchService is not going to consume the message. So the AuctionService and SearchService are consistent.
4. AuctionService is up, AuctionServiceDB is up, SearchService is up, SearchServiceDB is down, EventBus is up. = In this case AuctionService will createa auction and publish to the event bus, SearchService will get the message and try to update SearchServiceDB but as the DB is down operation will fail, therefore this will make the `two databases inconsistent`.
5. what if the servicebus is down? Yes, it''l be **inconsistent**.

`Data inconsistency` is one the big challenges of microservices applicaitons, beacuse we need to think about what do we need to do in certain scenarios and we need to think abut tha pretty much every mssage and you need to have a plan for each scenario.

## What are the options for failure?

MassTransit provides an outbox pattern and retry mechanisms that can help handle failures and ensure message delivery in a microservices architecture. Here's how you can leverage these features:

Outbox Pattern
The outbox pattern ensures that messages are reliably stored and eventually delivered, even if the service or the message broker experiences failures. This pattern typically involves storing messages in a local database table (the outbox) as part of the same transaction that modifies the business data. A background process then reads the messages from the outbox and sends them to the message broker.

Retry Mechanism
MassTransit provides built-in support for retry policies, allowing you to automatically retry message delivery in case of transient failures.
