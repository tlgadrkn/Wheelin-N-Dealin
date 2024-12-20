using System;
using System.Text.Json;
using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Entities;

namespace SearchService.Data;

public class DbInitializer
{

    public static async Task InitializeDb(WebApplication app) {

await DB.InitAsync("SearchDb", MongoClientSettings.FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

await DB.Index<Item>()
        .Key(s => s.Make, KeyType.Text)
        .Key(s => s.Model, KeyType.Text)
        .Key(s => s.Color, KeyType.Text)
        .CreateAsync();   
         
var count = await DB.CountAsync<Item>();

if (count > 0) {
    Console.WriteLine("Already have data - no seeding required");
    return;
} 

    Console.WriteLine("Seeding data...");
    var data = await File.ReadAllTextAsync("Data/auctions.json");

    var options = new JsonSerializerOptions{ PropertyNameCaseInsensitive = true};

    var items = JsonSerializer.Deserialize<List<Item>>(data, options);

    await DB.SaveAsync(items);

    Console.WriteLine("Data seeded");
    }
    

}
