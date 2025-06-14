﻿using Abstractions.Persistence;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Exceptions;
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
                          });
        
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
        
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        
        return app;
    }
}
