using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

using ScheduleJobs.Worker.Jobs.Models;
using ScheduleJobs.Worker.Repository;

namespace ScheduleJobs.Worker
{
    public class Worker : BackgroundService
    {
        public IScheduler Scheduler { get; set; }
        private readonly IJobFactory _jobFactory;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobRepository _jobRepository;
        private CancellationToken _cancellationToken;
        private readonly ILogger<Worker> _logger;

        public Worker(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IJobRepository jobRepository, ILogger<Worker> logger)
        {
            _jobFactory = jobFactory;
            _schedulerFactory = schedulerFactory;
            _jobRepository = jobRepository;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            //Creating Schdeular
            Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = _jobFactory;

            var timer = new System.Timers.Timer(5000);
            timer.Elapsed += AddJobs;
            timer.Start();

            await Scheduler.Start(_cancellationToken);
        }

        private void AddJobs(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<JobMetadata> jobs = _jobRepository.GetAsync().Result;

            foreach (var jobMetadata in jobs)
            {
                if (!JobExists(jobMetadata.JobId))
                {
                    IJobDetail jobDetail = CreateJob(jobMetadata);
                    ITrigger trigger = CreateTrigger(jobMetadata);
                    Scheduler.ScheduleJob(jobDetail, trigger, _cancellationToken).GetAwaiter();
                }
            }
        }

        private bool JobExists(string id)
        {
            bool hasJob = false;
            var jobGroups = Scheduler.GetJobGroupNames().Result;

            foreach (var group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = Scheduler.GetJobKeys(groupMatcher).Result;

                if (jobKeys.FirstOrDefault(j => j.Name == id) is not null)
                {
                    hasJob = true;
                    break;
                }
            }

            return hasJob;
        }

        private static ITrigger CreateTrigger(JobMetadata jobMetadata)
        {
            return TriggerBuilder.Create()
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithCronSchedule(jobMetadata.CronExpression)
                .WithDescription(jobMetadata.JobName)
                .Build();
        }

        private static IJobDetail CreateJob(JobMetadata jobMetadata)
        {
            return JobBuilder.Create(jobMetadata.JobType)
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithDescription(jobMetadata.JobName)
                .Build();
        }
    }
}