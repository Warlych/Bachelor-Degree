using Apis.Gateway.ExceptionHandlers;
using Asp.Versioning;
using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Ingest.Elasticsearch.DataStreams;
using Elastic.Serilog.Sinks;
using Metrics.Contracts.Grpc.Impl.Metrics;
using Microsoft.OpenApi.Models;
using RailwaySections.Contracts.Grpc.Impl.RailwaySections;
using Serilog;
using Serilog.Exceptions;
using Swashbuckle.AspNetCore.SwaggerGen;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Apis.Gateway;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<MetricsMicroservice.MetricsMicroserviceClient>(x =>
        {
            x.Address = new Uri(builder.Configuration["Microservices:Metrics"]);
        });
        
        builder.Services.AddGrpcClient<RailwaySectionsMicroservice.RailwaySectionsMicroserviceClient>(x =>
        {
            x.Address = new Uri(builder.Configuration["Microservices:RailwaySections"]);
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
                                        x.BootstrapMethod = BootstrapMethod.Silent;
                                    });

        });
        
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("allow-all-origins", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });
        
        builder.Services.AddControllers();
        
        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new QueryStringApiVersionReader("version"),
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-API-Version"),
                new MediaTypeApiVersionReader("version")
            );
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        
        builder.Services.AddEndpointsApiExplorer();
        
        builder.Services.AddProblemDetails(x =>
        {
            x.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
            };
        });

        builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();

        builder.Services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1",
                         new OpenApiInfo
                         {
                             Version = "1.0",
                             Title = "API v1"
                         });

            x.SwaggerDoc("v2",
                         new OpenApiInfo
                         {
                             Version = "2.0",
                             Title = "API v1"
                         });
        });
        
        return builder;
    }
    
    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCors("allow-all-origins");
        
        app.UseExceptionHandler();
        
        app.UseHsts();
        app.UseRouting();
        app.MapControllers();

        return app;
    }
}
