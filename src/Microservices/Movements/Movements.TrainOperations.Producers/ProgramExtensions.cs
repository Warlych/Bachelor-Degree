using Messages.Broker;
using Messages.Broker.Abstractions.Producers;
using Movements.Contracts.Contracts;
using Movements.TrainOperations.Producers.BackgroundServices;
using Movements.TrainOperations.Producers.Producers;

namespace Movements.TrainOperations.Producers;

public static class ProgramExtensions
{
    public static WebApplicationBuilder ConfigureWebApplication(this WebApplicationBuilder builder)
    {
        builder.Services.AddMessageBroker(builder.Configuration);

        builder.Services.AddOptions<TrainOperationPublisherOptions>()
               .Bind(builder.Configuration.GetSection("TrainOperationPublisher"))
               .ValidateDataAnnotations();
        
        builder.Services.AddScoped<TrainOperationDbProducer>();
        builder.Services.AddScoped<IProducer<TrainOperationCreatedEvent>>(sp => sp.GetRequiredService<TrainOperationDbProducer>());
        builder.Services.AddHostedService<TrainOperationPublisherService>();

        return builder;
    }
}
