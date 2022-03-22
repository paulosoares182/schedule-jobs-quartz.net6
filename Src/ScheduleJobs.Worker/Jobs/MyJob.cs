using Quartz;

namespace ScheduleJobs.Worker.Jobs
{
    [DisallowConcurrentExecution]
    public class MyJob : IJob
    {
        private readonly ILogger<MyJob> _logger;
        public MyJob(ILogger<MyJob> logger)
        {
            this._logger = logger;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            string message = $"\tExecuting Job '{context.JobDetail.Description}' - [{context.JobDetail.Key.Name}] at {DateTime.Now}\n";

            _logger.LogInformation(message);

            Random random = new(1000);
            await Task.Delay(random.Next(10000));
        }
    }
}