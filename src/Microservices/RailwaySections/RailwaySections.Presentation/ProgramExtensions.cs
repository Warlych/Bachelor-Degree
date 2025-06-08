using Abstractions.Persistence;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;
using RailwaySections.Domain.RailwaySections.Repositories;
using RailwaySections.Persistence;
using RailwaySections.Persistence.Abstractions;
using RailwaySections.Persistence.Repositories;
using RailwaySections.Persistence.Settings;
using RailwaySections.Presentation.Controllers.Grpc;
using Serilog;
using Serilog.Exceptions;

namespace RailwaySections.Presentation;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddDomain(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IRailwaySectionRepository, RailwaySectionRepository>();
        
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
        var options = new Neo4JOptions();
        builder.Configuration.GetSection(Neo4JOptions.SectionName).Bind(options);

        builder.Services.AddSingleton<IDriver>(GraphDatabase.Driver(options.Neo4JConnection, 
                                                                    AuthTokens.Basic(options.Neo4JUser, 
                                                                                     options.Neo4JPassword)));
        
        builder.Services.AddScoped<IDatabaseContext, DatabaseContext>();
        builder.Services.AddScoped<IRailwaySectionDatabaseContext, DatabaseContext>();
        
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

        builder.Services.AddHealthChecks();
        
        return builder;
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
