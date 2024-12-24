using SearchService.Data;
using SearchService.Services;
using MassTransit;
using SearchService.Consumers;
using System.Net;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ISearchService, SearchService.Services.SearchService>();
builder.Services.AddControllers();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<AuctionSvcHttpClient>().AddPolicyHandler(GetPolicy());
// builder.Services.AddMassTransit(x =>
// {
//     x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

//     x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

//     x.UsingRabbitMq((context, cfg) =>
//     {
//         cfg.ReceiveEndpoint("search-auction-created", e =>
//         {
//             e.UseMessageRetry(r => r.Interval(5,5));

//             e.ConfigureConsumer<AuctionCreatedConsumer>(context);
//         });

//         cfg.ConfigureEndpoints(context);
//     });
// });

builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AuctionCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.ReceiveEndpoint("search-auction-created", e =>
        {
            e.UseMessageRetry(r => r.Interval(5,5));

            e.ConfigureConsumer<AuctionCreatedConsumer>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

app.Lifetime.ApplicationStarted.Register(async () => {

try {

await DbInitializer.InitDb(app);

} catch(Exception ex) {
    Console.WriteLine($"Error initializing database: {ex.Message}");
}
});

app.Run();

// retry policy, polling AuctionService to get items for search db
// In real worl, probably would've used ElasticSearch instead of MongoDB for searching.
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
 
