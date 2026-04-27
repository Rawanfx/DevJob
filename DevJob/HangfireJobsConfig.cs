using DevJob.Application.ServiceContract;
using DevJob.Infrastructure.Data;
using Hangfire;
using Microsoft.EntityFrameworkCore;
public static class HangfireJobsConfig
{
    public static async Task RegisterRecurringJobs(this IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var searchKey = await context.SearchKeyWords.Where(x => x.IsActive).Select(x => x.Name).ToListAsync();
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

                recurringJobManager.AddOrUpdate<IBackgroundService>(
                    "Scraping-Jobs-Task",

                service => service.GetNewJobsFromSerpApi(),
                    Cron.HourInterval(12)
                );

                recurringJobManager.AddOrUpdate<IBackgroundService>(
                    "AI-Matching-Task",
                    service => service.CalculateMatchJobs(),
                    Cron.HourInterval(12)
                );
            }
        }
    }
}