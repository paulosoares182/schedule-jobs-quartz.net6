using ScheduleJobs.WebAPI.Jobs;
using ScheduleJobs.WebAPI.Jobs.Models;

namespace ScheduleJobs.WebAPI.Repository
{
    public static class FakeDB
    {
        static readonly List<JobMetadata> _jobs = new()
        {
            new JobMetadata(Guid.NewGuid().ToString(), typeof(MyJob), "Test Job", "0/15 * * * * ?")
        };

        public static void Insert(JobMetadata jobMetadata)
            => _jobs.Add(jobMetadata);

        public static List<JobMetadata> Get()
            => _jobs;
    }
}