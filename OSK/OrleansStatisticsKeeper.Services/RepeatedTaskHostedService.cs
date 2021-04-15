using Hangfire;
using Microsoft.Extensions.Hosting;
using OrleansStatisticsKeeper.Client.Services.Settings;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hangfire.Storage;

namespace OrleansStatisticsKeeper.Client.Services
{
    public abstract class RepeatedTaskHostedService : IHostedService
    {
        private readonly SchedulerSettings _settings;
        protected TimeSpan? DeltaTime;
        
        public RepeatedTaskHostedService(SchedulerSettings settings)
            => _settings = settings;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            RecurringJob.AddOrUpdate("cronWork",
                 () => InnerTask(cancellationToken),
                _settings.Schedule);

            RecurringJob.Trigger("cronWork");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }

        public virtual void InnerTask(CancellationToken cancellationToken)
        {
            var job = JobStorage.Current.GetConnection().GetRecurringJobs()
                .FirstOrDefault(p => p.Id == "cronWork");
            if (job == null) 
                return;
            job.LastExecution ??= DateTime.UtcNow;
            DeltaTime = job?.NextExecution - job?.LastExecution;
        }
    }
}
