namespace ScheduleJobs.Worker.Jobs.Models
{
    public class JobMetadata
    {
        public JobMetadata(string Id, Type jobType, string jobName, string cronExpression)
        {
            JobId = Id;
            JobType = jobType;
            JobName = jobName;
            CronExpression = cronExpression;
        }

        public string JobId { get; set; }
        public Type JobType { get; }
        public string JobName { get; }
        public string CronExpression { get; }
    }
}