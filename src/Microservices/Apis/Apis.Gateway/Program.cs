using Apis.Gateway;

var app = WebApplication.CreateBuilder(args)
                        .AddInfrastructure()
                        .AddPresentation()
                        .Build();

await app.AddMiddlewares()
         .RunAsync();
