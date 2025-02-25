using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NCrontab;

namespace Infrastructure.Base
{
    public abstract class BaseBackgroundWorker : BackgroundService
    {
        protected string ServiceName { get; set; }
        protected string CronExpression { get { return cronExpression; } set { cronExpression = value; UpdateCronExpression(); } }
        private string cronExpression;

        protected IServiceScopeFactory ScopeFactory { get; set; }
        protected CrontabSchedule Scheduler { get; set; }
        protected DateTime NextRun { get; set; }

        public BaseBackgroundWorker(IServiceScopeFactory scopeFactory, string serviceName)
        {
            ScopeFactory = scopeFactory;
            ServiceName = serviceName;
        }

        public BaseBackgroundWorker(IServiceScopeFactory scopeFactory, string serviceName, string cronExpression) : this(scopeFactory, serviceName)
        {
            this.cronExpression = cronExpression;
            Scheduler = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            NextRun = Scheduler.GetNextOccurrence(DateTime.UtcNow);
        }

        protected abstract void RunIteration();
        protected virtual void WarmUp() { }
        protected virtual void HandleIterationError() { }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (DateTime.UtcNow > NextRun)
                    {
                        WarmUp();

                        try
                        {
                            RunIteration();
                        }
                        catch (Exception exception)
                        {
                            HandleIterationError();
                        }

                        NextRun = Scheduler.GetNextOccurrence(DateTime.UtcNow);
                    }
                    Thread.Sleep(1000);
                }
            }, cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => { });
        }

        private void UpdateCronExpression()
        {
            Scheduler = CrontabSchedule.Parse(cronExpression, new CrontabSchedule.ParseOptions { IncludingSeconds = true });
            NextRun = Scheduler.GetNextOccurrence(DateTime.UtcNow);
        }
    }
}