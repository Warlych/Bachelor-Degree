using Abstractions.Persistence;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using Metrics.Domain.Metrics.Repositories;
using Metrics.Persistence;
using Metrics.Persistence.Abstractions;
using Metrics.Persistence.Repositories;
using Metrics.Presentation.Controllers.Grpc;
using Microsoft.EntityFrameworkCore;
using Movements.Contracts.Grpc.Impl.Movements;
using RailwaySections.Contracts.Grpc.Impl.RailwaySections;
using Serilog;
using Serilog.Exceptions;
using Trains.Contracts.Grpc.Impl.Trains;

namespace Metrics.Presentation;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IMetricsRepository, MetricsRepository>();
        
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
        
        builder.Services.AddScoped<IMetricDatabaseContext, DatabaseContext>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<RailwaySectionsMicroservice.RailwaySectionsMicroserviceClient>(x =>
        {
            x.Address = new Uri(builder.Configuration["Microservices:RailwaySections"]);
        });
        
        builder.Services.AddGrpcClient<MovementsMicroservice.MovementsMicroserviceClient>(x =>
        {
            x.Address = new Uri(builder.Configuration["Microservices:Movements"]);
        });
        
        builder.Services.AddGrpcClient<TrainsMicroservice.TrainsMicroserviceClient>(x =>
        {
            x.Address = new Uri(builder.Configuration["Microservices:Trains"]);
        });
        
        return builder;
    }
    
    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog(x =>
        {
            x.MinimumLevel.Debug()
             .Enrich.FromLogContext()
             .Enrich.WithExceptionDetails()
             .WriteTo.Console()
             .WriteTo.Elasticsearch(new[]
                                    {
                                        new Uri(builder.Configuration["ElasticConfiguration:Uri"])
                                    },
                                    x =>
                                    {
                                        x.BootstrapMethod = BootstrapMethod.Failure;
                                    });

        });
        
        builder.Services.AddGrpc();
        
        return builder;
    }

    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        
        var context = scope.ServiceProvider.GetRequiredService<IMetricDatabaseContext>();
        context.Database.Migrate();

        return app;
    }
    
    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        app.MapGrpcService<GrpcService>();
        
        return app;
    }
}