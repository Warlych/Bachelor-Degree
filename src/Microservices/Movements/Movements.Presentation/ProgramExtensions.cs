using Abstractions.Persistence;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using HealthChecks.UI.Client;
using Mediator;
using Messages.Broker;
using Messages.Broker.Abstractions.Consumers;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Movements.Contracts.Contracts;
using Movements.Domain.TrainOperations.Repositories;
using Movements.Persistence;
using Movements.Persistence.Abstractions;
using Movements.Persistence.Repositories;
using Movements.Presentation.Consumers;
using Movements.Presentation.Controllers.Grpc;
using RabbitMQ.Client;
using Serilog;
using Serilog.Exceptions;

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
                                        x.BootstrapMethod = BootstrapMethod.Silent;
                                    });

        });
        
        builder.Services.AddScoped<IConsumer<TrainOperationCreatedEvent>, TrainOperationCreatedEventConsumer>();
        builder.Services.AddScoped<IConsumer, TrainOperationCreatedEventConsumer>();
        
        builder.Services.AddMessageBroker(builder.Configuration);
        
        builder.Services.AddGrpc();

        builder.Services
               .AddHealthChecks()
               .AddNpgSql(connectionString: builder.Configuration["ConnectionStrings:Default"],
                          name: "postgresql",
                          tags: new[]
                          {
                              "db",
                              "sql",
                              "postgres"
                          })
               .AddRabbitMQ(async x =>
                            {
                                return await x.GetService<IConnectionFactory>().CreateConnectionAsync();
                            },
                            name: "rabbitmq",
                            tags: new[]
                            {
                                "queue",
                                "rabbitmq"
                            });
        
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
        
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        
        return app;
    }
}
