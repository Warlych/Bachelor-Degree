using Messages.Broker.Abstractions.Bus;
using Messages.Broker.Abstractions.Consumers;
using Messages.Broker.Abstractions.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Messages.Broker;

public static class DependencyInjection
{
    public static IServiceCollection AddMessageBroker(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionFactory>(x =>
        {
            return new ConnectionFactory
            {
                Uri = new Uri(configuration["RabbitMQ:Uri"] ?? "amqp://admin:admin@localhost:5672/"),
            };
        });
        
        services.AddSingleton<IBus, Bus>();
        services.AddHostedService<BusHostedService>();
        
        return services;
    }

}
