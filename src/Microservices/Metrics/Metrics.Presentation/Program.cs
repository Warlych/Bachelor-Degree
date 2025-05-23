using Metrics.Presentation;

var app = WebApplication.CreateBuilder(args)
                        .AddDomain()
                        .AddApplication()
                        .AddPersistence()
                        .AddInfrastructure()
                        .AddPresentation()
                        .Build();

await app.MigrateDatabase()
         .AddMiddlewares()
         .RunAsync();
         