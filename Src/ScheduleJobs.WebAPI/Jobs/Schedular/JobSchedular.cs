using Quartz;
using Quartz.Impl.Matchers;
using Quartz.Spi;

using ScheduleJobs.WebAPI.Jobs.Models;
using ScheduleJobs.WebAPI.Repository;

namespace ScheduleJobs.WebAPI.Jobs.Schedular
{
    public class JobSchedular : IHostedService
    {
        public IScheduler Scheduler { get; set; }
        private readonly IJobFactory _jobFactory;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobRepository _jobRepository;
        private CancellationToken _cancellationToken;

        public JobSchedular(ISchedulerFactory schedulerFactory, IJobFactory jobFactory, IJobRepository jobRepository)
        {
            _jobFactory = jobFactory;
            _schedulerFactory = schedulerFactory;
            _jobRepository = jobRepository;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            //Creating Schdeular
            Scheduler = await _schedulerFactory.GetScheduler();
            Scheduler.JobFactory = _jobFactory;

            var timer = new System.Timers.Timer(5000);
            timer.Elapsed += AddJobs;
            timer.Start();

            await Scheduler.Start(_cancellationToken);
        }

        private void AddJobs(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<JobMetadata> jobs = _jobRepository.GetAsync().Result;

            foreach(var jobMetadata in jobs)
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

            foreach(var group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = Scheduler.GetJobKeys(groupMatcher).Result;

                if(jobKeys.FirstOrDefault(j => j.Name == id) is not null)
                {
                    hasJob = true;
                    break;
                }
            }

            return hasJob;
        }

        private ITrigger CreateTrigger(JobMetadata jobMetadata)
        {
            return TriggerBuilder.Create()
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithCronSchedule(jobMetadata.CronExpression)
                .WithDescription(jobMetadata.JobName)
                .Build();
        }

        private IJobDetail CreateJob(JobMetadata jobMetadata)
        {
            return JobBuilder.Create(jobMetadata.JobType)
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithDescription(jobMetadata.JobName)
                .Build();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Scheduler.Shutdown(cancellationToken);
        }
    }
}
