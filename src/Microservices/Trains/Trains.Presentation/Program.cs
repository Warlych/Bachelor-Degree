using Trains.Presentation;

var app = WebApplication.CreateBuilder(args)
                        .AddDomain()
                        .AddApplication()
                        .AddPersistence()
                        .AddPresentation()
                        .Build();

await app.MigrateDatabase()
         .AddMiddlewares()
         .RunAsync();
