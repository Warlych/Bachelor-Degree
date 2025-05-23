using Microsoft.Extensions.Options;
using Movements.TrainOperations.Producers.Producers;

namespace Movements.TrainOperations.Producers.BackgroundServices;

/// <summary>
/// Настройки сервиса публикации операций с поездами
/// </summary>
public class TrainOperationPublisherOptions
{
    /// <summary>
    /// Интервал публикации в секундах
    /// </summary>
    public int IntervalSeconds { get; set; } = 60;
    
    /// <summary>
    /// За сколько последних минут публиковать данные
    /// </summary>
    public int LastMinutes { get; set; } = Int32.MaxValue;
    
    /// <summary>
    /// Автоматически запускать сервис при старте приложения
    /// </summary>
    public bool AutoStart { get; set; } = true;
}

/// <summary>
/// Фоновый сервис для периодической публикации событий операций с поездами из БД
/// </summary>
public class TrainOperationPublisherService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TrainOperationPublisherService> _logger;
    private readonly TrainOperationPublisherOptions _options;

    public TrainOperationPublisherService(IServiceProvider serviceProvider,
                                          IOptions<TrainOperationPublisherOptions> options,
                                          ILogger<TrainOperationPublisherService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.AutoStart)
        {
            _logger.LogInformation("Auto start is disabled");
            
            return;
        }

        _logger.LogInformation("Publish was started. Interval: {IntervalSeconds} sec., Last start: {LastMinutes} min.",
                               _options.IntervalSeconds,
                               _options.LastMinutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var producer = scope.ServiceProvider.GetRequiredService<TrainOperationDbProducer>();

                var publishedCount = await producer.PublishLatestOperationsAsync(_options.LastMinutes,
                                                                                 stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish");
            }

            await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
        }
    }
}
