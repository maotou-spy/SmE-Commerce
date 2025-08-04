using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceServices.BackgroundTaskService;

public class OrderAutoCompleteBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<OrderAutoCompleteBackgroundService> logger
) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("OrderAutoCompleteBackgroundService is starting...");

        while (!stoppingToken.IsCancellationRequested)
            try
            {
                // Calculate the time until midnight
                var timeUntilMidnight = DateTime.Today.AddDays(1) - DateTime.Now;

                logger.LogInformation(
                    "OrderAutoCompleteBackgroundService will be executed at midnight. Waiting for {TimeToMidnight}...",
                    timeUntilMidnight.ToString(@"hh\:mm\:ss")
                );

                // Wait until midnight
                await Task.Delay(timeUntilMidnight, stoppingToken);

                // Auto complete orders in midnight
                logger.LogInformation("OrderAutoCompleteBackgroundService is executing...");
                await ProcessOrderAutoComplete();

                logger.LogInformation(
                    "OrderAutoCompleteBackgroundService completed in {Time}",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                );
            }
            catch (OperationCanceledException)
            {
                // Application is stopping
                logger.LogInformation("OrderAutoCompleteBackgroundService is stopping...");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Error in OrderAutoCompleteBackgroundService: {Message}",
                    ex.Message
                );
                // If has an error, wait for 1 hour before retrying
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
    }

    private async Task ProcessOrderAutoComplete()
    {
        // Create a scope to resolve services
        using var scope = serviceProvider.CreateScope();

        // Get from app settings the number of days after which orders should be auto completed
        var autoCompleteDays = int.Parse(
            scope
                .ServiceProvider.GetRequiredService<IConfiguration>()
                .GetSection("BackgroundServices:OrderAutoComplete:AutoCompleteDays")
                .Value ?? "10"
        );

        // Get the IOrderService from the scope
        var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

        // Call the method to auto complete shipped orders
        await orderService.SystemAutoCompleteShippedOrdersAsync(autoCompleteDays);
    }
}
