using System.Diagnostics;
using Mediator;
using Microsoft.Extensions.Logging;
using RailwaySections.Domain.RailwaySections.Repositories;

namespace RailwaySections.Application.Handlers.Commands.Graph.Build;

public record BuildGraphCommand : ICommand<bool>;

public sealed class BuildGraphCommandHandler : ICommandHandler<BuildGraphCommand, bool>
{
    private readonly IRailwaySectionRepository _railwaySectionRepository;
    private readonly ILogger<BuildGraphCommandHandler> _logger;

    public BuildGraphCommandHandler(IRailwaySectionRepository railwaySectionRepository, ILogger<BuildGraphCommandHandler> logger)
    {
        _railwaySectionRepository = railwaySectionRepository;
        _logger = logger;
    }

    public async ValueTask<bool> Handle(BuildGraphCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var stopwatch = new Stopwatch();

            _logger.LogInformation("Build graph command has started");

            stopwatch.Start();

            await _railwaySectionRepository.BuildGraphAsync(cancellationToken);

            stopwatch.Stop();

            _logger.LogInformation("Builder graph command has finished. Total time: {time}", stopwatch.Elapsed);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to build graph command");
            
            throw;
        }
    }
}
