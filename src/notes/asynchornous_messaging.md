# Asynchronous Messaging

When we want to decouple services, we can use asynchronous messaging, it enables services to communicate without waiting for a response.

So what if the service is down? The message will be lost? No, the message will be stored in a message broker, and the service will consume it when it's up again.

So we have an Event Bus, which is a message broker, and we have Producers and Consumers. Producers send messages to the Event Bus, and Consumers consume messages from the Event Bus.

## What if Event Bus is down?

If Event bus is down, for sure our other services will not be able to communicate each other as they'll be reliant on the Event Bus.

But if we design our microservices well enough, they can still do the jobs they're designed to do they can receive and respond to any request that come externally to that service. They simply just cannot communicate with each other until event bus is up again.

Event bus should be treated as a first class citizen in our architecture, it should be highly available, resilient, fault tolerant, should have persisted storage so if it goes down it can continue the persisted messages when it comes up again. And each of our services should handle a retry mechanism to send messages to the event bus, so they can retrive the messages when the event bus is up again.

## RabbitMQ

RabbitMq is a message broker, it is a software where we can define queues, exchanges, and bindings. It is a message broker that implements the Advanced Message Queuing Protocol (AMQP). It acts as a middleman for various services to communicate with each other.

- basically, it is a message broker that allows us to send and receive messages between services. accept messages from producers, and forward message to the queue where consumers can consume them.

- It uses Producer-Consumer model (Pub/Sub)
- Messages are stored in queues and it uses a **message buffer**, which is a temporary storage area for messages that are waiting to be delivered to consumers.
- Rabitmq can have persistent queues, so if the server goes down, the messages will be persisted and will be available when the server comes up again because of message buffer.
- It uses **Exchanges** to route messages to the correct queue. Exchanges are the routing mechanism for RabbitMQ. It receives messages from producers and routes them to the **correct queue based** on the **routing key**.
- It uses AMQP protocol, which is a standard protocol for messaging.
- **Mass Transit** is a library that makes it easier to work with RabbitMQ in .NET, its basically the Entity Framework for RabbitMQ.

### Exchanges

There are 4 types of exchanges in RabbitMQ:

1. **Direct Exchange**: A direct exchange delivers messages to queues based on a message routing key. It is useful when we want to deliver messages to a specific queue.
2. **Fanout Exchange**: A fanout exchange delivers messages to all queues that are bound to it. It is useful when we want to deliver messages to all queues. So our publisher will publish a message to exchange, This exchange has one or more queues bound to it. The exchange will deliver the message to all the queues that are bound to it.
3. **Topic Exchange**: A topic exchange delivers messages to queues based on a pattern (routing key). It is useful when we want to deliver messages to multiple queues based on a pattern.
4. **Headers Exchange**: A headers exchange delivers messages based on message headers. It is useful when we want to deliver messages based on message headers.

## Data Consistency

When we have multiple services, we need to ensure that the data is consistent across all services.

So we need to plan for eventual consistency, which means that the data will be consistent eventually, but not immediately.

we need to think what happens if one service fails, how do we recover from that failure, how do we ensure that the data is consistent across all services.

So using a message broker like RabbitMQ, we can ensure that the data is consistent across all services. Combined with Mass Transit, we can ensure that the data is consistent across all services using outbox pattern.

## Outbox Pattern
