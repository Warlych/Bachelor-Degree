using Mediator;
using Messages.Broker.Abstractions.Consumers;
using Messages.Broker.Common.Attributes;
using Movements.Application.Handlers.Commands.CreateTrainOperation;
using Movements.Contracts.Contracts;
using Movements.Domain.TrainOperations.Enums;
using Movements.Domain.TrainOperations.ValueObjects;

namespace Movements.Presentation.Consumers;

/// <summary>
/// Консьюмер для обработки событий об операциях с поездами
/// </summary>
[Queue("train-operations", true)]
public sealed class TrainOperationCreatedEventConsumer : IConsumer<TrainOperationCreatedEvent>
{
    private readonly ISender _sender;
    private readonly ILogger<TrainOperationCreatedEventConsumer> _logger;

    public TrainOperationCreatedEventConsumer(ISender sender, ILogger<TrainOperationCreatedEventConsumer> logger)
    {
        _sender = sender;
        _logger = logger;
    }

    public async ValueTask ConsumeAsync(TrainOperationCreatedEvent message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Received train operation event: ID={OperationId}, Code={Code}, "
                                   + "Train={TrainNumber}, Route: {FromSection} -> {ToSection}, Time: {TimeStamp}",
                                   message.Id,
                                   message.Code,
                                   message.Train?.Number,
                                   message.From?.UnifiedNetworkMarking,
                                   message.To?.UnifiedNetworkMarking,
                                   message.TimeStamp);

            if (!Enum.IsDefined(typeof(TrainOperationCodes), message.Code))
            {
                _logger.LogWarning("Unknown train operation code: {Code}", message.Code);
                return;
            }


            await _sender.Send(new CreateTrainOperationCommand(new ExternalIdentifier(message.Train?.Number),
                                                               new ExternalIdentifier(message.From?.UnifiedNetworkMarking),
                                                               new ExternalIdentifier(message.To?.UnifiedNetworkMarking),
                                                               (TrainOperationCodes)message.Code,
                                                               message.TimeStamp.ToUniversalTime()),
                               cancellationToken);
            
            _logger.LogInformation("CreateTrainOperationCommand sent successfully for OperationId={OperationId}", message.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process: {OperationId}", message.Id);

            throw;
        }
    }
}
