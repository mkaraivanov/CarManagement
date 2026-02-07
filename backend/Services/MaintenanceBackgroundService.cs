using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Backend.Models;

namespace Backend.Services
{
    /// <summary>
    /// Background service that runs daily to check for upcoming/overdue maintenance
    /// and creates reminders and notifications
    /// </summary>
    public class MaintenanceBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MaintenanceBackgroundService> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Run daily

        public MaintenanceBackgroundService(
            IServiceProvider serviceProvider,
            ILogger<MaintenanceBackgroundService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Maintenance Background Service is starting");

            // Wait for initial delay (run at midnight UTC)
            await WaitForNextScheduledRun(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Maintenance Background Service is running scheduled check");

                    await PerformMaintenanceCheckAsync();

                    _logger.LogInformation("Maintenance Background Service completed scheduled check");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in Maintenance Background Service");
                }

                // Wait for next interval
                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Maintenance Background Service is stopping");
        }

        private async Task PerformMaintenanceCheckAsync()
        {
            using var scope = _serviceProvider.CreateScope();

            var reminderService = scope.ServiceProvider.GetRequiredService<IReminderService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            try
            {
                // Step 1: Check all maintenance schedules and create reminders for upcoming/overdue items
                _logger.LogInformation("Checking maintenance schedules for reminders...");
                await reminderService.CheckAndCreateRemindersAsync();

                // Step 2: Get pending reminders and create notifications for them
                _logger.LogInformation("Creating notifications from reminders...");
                var reminders = await reminderService.GetRemindersForUserAsync(Guid.Empty); // This needs to be updated

                // TODO: Get all users' pending reminders instead
                // For now, we'll create in-app notifications from reminders via the API when reminders are created

                // Step 3: Send pending notifications
                _logger.LogInformation("Sending pending notifications...");
                await notificationService.SendPendingNotificationsAsync();

                // Step 4: Cleanup old reminders and notifications
                _logger.LogInformation("Cleaning up old reminders and notifications...");
                await reminderService.DeleteOldRemindersAsync(90);
                await notificationService.DeleteOldNotificationsAsync(30);

                _logger.LogInformation("Maintenance check completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during maintenance check");
                throw;
            }
        }

        private async Task WaitForNextScheduledRun(CancellationToken stoppingToken)
        {
            var now = DateTime.UtcNow;
            var nextMidnight = now.Date.AddDays(1); // Next midnight UTC
            var delay = nextMidnight - now;

            _logger.LogInformation($"Waiting {delay.TotalHours:F2} hours until first scheduled run at {nextMidnight} UTC");

            await Task.Delay(delay, stoppingToken);
        }
    }
}
