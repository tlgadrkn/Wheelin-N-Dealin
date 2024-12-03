using System.Text.Json.Serialization;
using AuctionService.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AuctionDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers()
.AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try {
    DbInitializer.InitDb(app);
} catch (Exception ex) {
    Console.WriteLine(ex.Message);
}

app.Run();
