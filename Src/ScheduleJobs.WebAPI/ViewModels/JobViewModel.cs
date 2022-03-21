namespace ScheduleJobs.WebAPI.ViewModels
{
    public class JobViewModel
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string? CronExpression { get; set; }
        public DateTime ScheduleDateTime { get; set; }
    }
}