using Movements.TrainOperations.Producers;

var app = WebApplication.CreateBuilder(args)
                        .ConfigureWebApplication()
                        .Build();

await app.RunAsync();
