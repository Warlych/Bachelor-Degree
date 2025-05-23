using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;
using RailwaySections.Application.Handlers.Commands.Graph.Build;
using RailwaySections.Domain.RailwaySections.Repositories;

namespace RailwaySections.Application.Handlers.Commands.Graph.Drop;

public record DropGraphCommand : ICommand<bool>;

public sealed class DropGraphCommandHandler : ICommandHandler<DropGraphCommand, bool>
{
    private readonly IRailwaySectionRepository _railwaySectionRepository;
    private readonly ILogger<BuildGraphCommandHandler> _logger;

    public DropGraphCommandHandler(IRailwaySectionRepository railwaySectionRepository, ILogger<BuildGraphCommandHandler> logger)
    {
        _railwaySectionRepository = railwaySectionRepository;
        _logger = logger;
    }

    public async ValueTask<bool> Handle(DropGraphCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var stopwatch = new Stopwatch();

            _logger.LogInformation("Drop graph command has started");

            stopwatch.Start();

            await _railwaySectionRepository.DropGraphIfExistsAsync(cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation("Drop graph command has finished. Total time: {time}", stopwatch.Elapsed);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to drop graph");
            
            throw;
        }
    }
}
