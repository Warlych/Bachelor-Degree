using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Messages.Broker.Abstractions.Bus;
using Messages.Broker.Abstractions.Producers;
using Messages.Broker.Common.Attributes;
using Messages.Broker.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Movements.Contracts.Contracts;
using Npgsql;

namespace Movements.TrainOperations.Producers.Producers;

/// <summary>
/// Продюсер для публикации событий об операциях с поездами из БД PostgreSQL
/// </summary>
[Queue("train-operations", true)]
public class TrainOperationDbProducer : Producer<TrainOperationCreatedEvent>
{
    private readonly string _connectionString;
    private readonly ILogger<TrainOperationDbProducer> _logger;

    public TrainOperationDbProducer(IBus bus,
                                    IConfiguration configuration,
                                    ILogger<TrainOperationDbProducer> logger) : base(bus)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("Default") ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<int> PublishLatestOperationsAsync(int lastMinutes = 5, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publist operation over the past {Minutes} minutes", lastMinutes);

        var publishedCount = 0;

        try
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            string sql = @"
                SELECT 
                    op.id_oper, 
                    op.vrsvop, 
                    op.nom_poezd, 
                    op.kodop_p,
                    op.dis_esr,
                    op.esr_napr_otp,
                    op.esr_napr_prib,
                    obj.brutto,
                    obj.udl
                FROM 
                    gid_storage.tm_object_op op
                LEFT JOIN 
                    gid_storage.tm_objects obj ON op.id_poezd = obj.id_poezd
                WHERE 
                    op.vrsvop >= (NOW() - INTERVAL '{0} MINUTES')
                ORDER BY 
                    op.vrsvop DESC";

            await using var command = new NpgsqlCommand(String.Format(sql, lastMinutes), connection);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                var operationEvent = MapToTrainOperationEvent(reader);

                if (operationEvent != null)
                {
                    _logger.LogDebug("Publish: {OperationId}, Train: {TrainNumber}, Operation code: {Code}",
                                     operationEvent.Id,
                                     operationEvent.Train?.Number,
                                     operationEvent.Code);

                    await PublishAsync(operationEvent, cancellationToken);

                    publishedCount++;
                }
            }

            _logger.LogInformation("Total publish: {Count}", publishedCount);

            return publishedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database executing error");
            throw;
        }
    }

    private TrainOperationCreatedEvent MapToTrainOperationEvent(IDataReader reader)
    {
        try
        {
            var idOper = reader.GetInt64(reader.GetOrdinal("id_oper"));
            var timestamp = reader.GetDateTime(reader.GetOrdinal("vrsvop"));
            var trainNumber = reader.IsDBNull(reader.GetOrdinal("nom_poezd"))
                                  ? String.Empty
                                  : reader.GetString(reader.GetOrdinal("nom_poezd"));

            var operationCode = reader.GetInt32(reader.GetOrdinal("kodop_p"));

            var stationEsr = reader.IsDBNull(reader.GetOrdinal("dis_esr"))
                                 ? String.Empty
                                 : reader.GetString(reader.GetOrdinal("dis_esr"));

            var fromEsr = reader.IsDBNull(reader.GetOrdinal("esr_napr_otp"))
                              ? null
                              : reader.GetString(reader.GetOrdinal("esr_napr_otp"));

            var toEsr = reader.IsDBNull(reader.GetOrdinal("esr_napr_prib"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("esr_napr_prib"));

            var resolvedFromEsr = String.IsNullOrWhiteSpace(fromEsr) ? stationEsr : fromEsr;
            var resolvedToEsr = String.IsNullOrWhiteSpace(toEsr) ? stationEsr : toEsr;

            if (String.IsNullOrWhiteSpace(resolvedFromEsr) || String.IsNullOrWhiteSpace(resolvedToEsr))
            {
                _logger.LogWarning("Missing ESR values for operation {OperationId}: from={FromEsr}, to={ToEsr}, fallback={StationEsr}",
                                   idOper,
                                   fromEsr,
                                   toEsr,
                                   stationEsr);
            }

            var trainOperationEvent = new TrainOperationCreatedEvent
            {
                Id = Guid.CreateVersion7(),
                Code = operationCode,
                TimeStamp = timestamp,
                Train = new Train
                {
                    Number = trainNumber
                },
                From = new RailwaySection
                {
                    UnifiedNetworkMarking = String.IsNullOrEmpty(fromEsr) ? stationEsr : fromEsr
                },
                To = new RailwaySection
                {
                    UnifiedNetworkMarking = String.IsNullOrEmpty(toEsr) ? stationEsr : toEsr
                }
            };

            return trainOperationEvent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed mapping train operation event");

            return null;
        }
    }
}
