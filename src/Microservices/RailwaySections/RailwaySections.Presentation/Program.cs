using RailwaySections.Presentation;

var app = WebApplication.CreateBuilder(args)
                        .AddDomain()
                        .AddApplication()
                        .AddPersistence()
                        .AddPresentation()
                        .Build();

await app.AddMiddlewares()
         .RunAsync();

