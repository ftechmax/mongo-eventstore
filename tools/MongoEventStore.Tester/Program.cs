using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoEventStore;
using MongoEventStore.Tester.DomainEvents;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Domain mappings
            BsonClassMap.RegisterClassMap<MyCreated>();
            BsonClassMap.RegisterClassMap<MyUpdated>();

            var connectionString = $"mongodb://safety-user:password@localhost:27017";
            builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

            builder.Services.AddMongoEventStore(config =>
            {
                config.DatabaseName = "MongoEventStore";
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}