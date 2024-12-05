
using SearchService.Data;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<ISearchService, SearchService.Services.SearchService>();
builder.Services.AddControllers();

var app = builder.Build();

app.UseAuthorization();

app.MapControllers();

// init mongodb

try {

await DbInitializer.InitializeDb(app);

} catch(Exception ex) {
    Console.WriteLine($"Error initializing database: {ex.Message}");
}

app.Run();
