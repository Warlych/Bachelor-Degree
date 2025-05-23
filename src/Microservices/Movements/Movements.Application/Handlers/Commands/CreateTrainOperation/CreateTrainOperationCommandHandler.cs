using Abstractions.Persistence;
using Mediator;
using Microsoft.Extensions.Logging;
using Movements.Domain.TrainOperations;
using Movements.Domain.TrainOperations.Enums;
using Movements.Domain.TrainOperations.Repositories;
using Movements.Domain.TrainOperations.ValueObjects;
using Movements.Domain.TrainOperations.ValueObjects.TrainOperations;

namespace Movements.Application.Handlers.Commands.CreateTrainOperation;

public record CreateTrainOperationCommand(ExternalIdentifier TrainIdentifier,
                                          ExternalIdentifier RailwaySectionFromIdentifier,
                                          ExternalIdentifier RailwaySectionToIdentifier,
                                          TrainOperationCodes Code,
                                          DateTime TimeStamp) 
    : ICommand<DateTime>;

public sealed class CreateTrainOperationCommandHandler : ICommandHandler<CreateTrainOperationCommand, DateTime>
{
    private readonly ITrainOperationRepository _trainOperationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateTrainOperationCommandHandler> _logger;

    public CreateTrainOperationCommandHandler(ITrainOperationRepository trainOperationRepository, IUnitOfWork unitOfWork, ILogger<CreateTrainOperationCommandHandler> logger)
    {
        _trainOperationRepository = trainOperationRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async ValueTask<DateTime> Handle(CreateTrainOperationCommand command, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            
            var trainOperation = TrainOperation.Create(command.TrainIdentifier,
                                                       command.RailwaySectionFromIdentifier,
                                                       command.RailwaySectionToIdentifier,
                                                       command.Code,
                                                       command.TimeStamp);

            await _trainOperationRepository.AddAsync(trainOperation, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken: cancellationToken);
            
            _logger.LogInformation("Created train operation with id: {Id}", trainOperation.Id);
            
            return trainOperation.TimeStamp;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            
            _logger.LogError(ex, "Failed to create train operation");
            
            throw;
        }
    }
}
