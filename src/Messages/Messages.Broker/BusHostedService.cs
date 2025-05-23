using Messages.Broker.Abstractions.Bus;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Messages.Broker;

public sealed class BusHostedService : IHostedService
{
    private readonly IBus _bus;
    private readonly ILogger<BusHostedService> _logger;

    public BusHostedService(IBus bus, ILogger<BusHostedService> logger)
    {
        _bus = bus;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting bus");
        
        await _bus.StartConsumeAsync(cancellationToken);
        
        _logger.LogInformation("Bus started");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping bus");

        await _bus.StopConsumeAsync(cancellationToken);
        
        _logger.LogInformation("Bus stopped");
    }
}
