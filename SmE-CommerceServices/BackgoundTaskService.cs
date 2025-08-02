using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmE_CommerceModels.Enums;
using SmE_CommerceRepositories.Interface;

namespace SmE_CommerceServices;

public class BackgoundTaskService(ILogger<BackgoundTaskService> logger, IOrderRepository orderRepository)
    : BackgroundService
{
    private readonly ILogger _logger = logger;
    private readonly IOrderRepository _orderRepository = orderRepository;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background task service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var tenDaysAgo = DateTime.UtcNow.AddDays(-10);
                var ordersResult = await _orderRepository.GetShippedOrdersOlderThanAsync(tenDaysAgo);

                if (ordersResult is { IsSuccess: true, Data: not null })
                {
                    foreach (var order in ordersResult.Data)
                    {
                        order.Status = OrderStatus.Completed;
                        order.ModifiedAt = DateTime.UtcNow;
                    }

                    await _orderRepository.UpdateOrderStatusRangeAsync(ordersResult.Data);
                    _logger.LogInformation("Updated shipped orders to completed.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in background task: {Error}", ex.Message);
            }

            var now = DateTime.Now;
            var nextRunTime = DateTime.Today.AddHours(10);

            if (now > nextRunTime)
            {
                nextRunTime = nextRunTime.AddDays(1);
            }

            var delay = nextRunTime - now;
            _logger.LogInformation("Next run scheduled at {Time}", nextRunTime);
            await Task.Delay(delay, stoppingToken);
        }

        _logger.LogInformation("Background task service stopped.");
    }
}