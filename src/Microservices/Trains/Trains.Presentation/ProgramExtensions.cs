using Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;
using Trains.Domain.Trains.Repositories;
using Trains.Persistence;
using Trains.Persistence.Abstractions;
using Trains.Persistence.Repositories;
using Trains.Presentation.Controllers.Grpc;

namespace Trains.Presentation;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ITrainRepository, TrainRepository>();
        
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
        
        builder.Services.AddScoped<ITrainDatabaseContext, DatabaseContext>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpc();
        
        return builder;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<ITrainDatabaseContext>();
        context.Database.Migrate();

        return app;
    }
    
    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        app.MapGrpcService<GrpcService>();
        
        return app;
    }
}
