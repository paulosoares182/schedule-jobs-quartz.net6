using ScheduleJobs.Worker.Jobs;
using ScheduleJobs.Worker.Jobs.Models;

namespace ScheduleJobs.Worker.Repository
{
    public static class FakeDB
    {
        static readonly List<JobMetadata> _jobs = new()
        {
            new JobMetadata(Guid.NewGuid().ToString(), typeof(MyJob), "Test Job", "0/5 * * * * ?")
        };

        public static void Insert(JobMetadata jobMetadata)
            => _jobs.Add(jobMetadata);

        public static List<JobMetadata> Get()
            => _jobs;
    }
}