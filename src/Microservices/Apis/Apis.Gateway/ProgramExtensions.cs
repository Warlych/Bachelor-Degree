using Asp.Versioning;
using Metrics.Contracts.Grpc.Impl.Metrics;

namespace Apis.Gateway;

public static class ProgramExtensions
{
    public static WebApplicationBuilder AddInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Services.AddGrpcClient<MetricsMicroservice.MetricsMicroserviceClient>(x =>
        {
            x.Address = new Uri(builder.Configuration["Microservices:Metrics"]);
        });
        
        return builder;
    }
    
    public static WebApplicationBuilder AddPresentation(this WebApplicationBuilder builder)
    {
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
        
        builder.Services.AddSwaggerGen();
        
        return builder;
    }
    
    public static WebApplication AddMiddlewares(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHsts();
        app.UseRouting();
        app.MapControllers();
        
        return app;
    }
}
