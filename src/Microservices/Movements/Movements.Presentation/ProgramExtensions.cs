using Abstractions.Persistence;
using Mediator;
using Messages.Broker;
using Messages.Broker.Abstractions.Consumers;
using Microsoft.EntityFrameworkCore;
using Movements.Contracts.Contracts;
using Movements.Domain.TrainOperations.Repositories;
using Movements.Persistence;
using Movements.Persistence.Abstractions;
using Movements.Persistence.Repositories;
using Movements.Presentation.Consumers;
using Movements.Presentation.Controllers.Grpc;

namespace Movements.Presentation;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITrainOperationRepository, TrainOperationRepository>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediator(config =>
        {
            config.ServiceLifetime = ServiceLifetime.Scoped;
        });
        
        return builder;
    }
    
    public static WebApplicationBuilder AddPersistence(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<IDatabaseContext, DatabaseContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
                   .UseSnakeCaseNamingConvention()
                   .EnableDetailedErrors()
                   .EnableSensitiveDataLogging()
                   .LogTo(Console.WriteLine,
                          LogLevel.Information);
        });
        
        builder.Services.AddScoped<ITrainOperationDatabaseContext, DatabaseContext>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IConsumer<TrainOperationCreatedEvent>, TrainOperationCreatedEventConsumer>();
        builder.Services.AddScoped<IConsumer, TrainOperationCreatedEventConsumer>();
        
        builder.Services.AddMessageBroker(builder.Configuration);
        
        builder.Services.AddGrpc();
        
        return builder;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ITrainOperationDatabaseContext>();
        context.Database.Migrate();

        return app;
    }
    
    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        app.MapGrpcService<GrpcService>();
        
        return app;
    }
}
