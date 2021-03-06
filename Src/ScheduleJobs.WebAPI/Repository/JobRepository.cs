using ScheduleJobs.WebAPI.Jobs.Models;

namespace ScheduleJobs.WebAPI.Repository
{
    public class JobRepository : IJobRepository
    {
        public async Task<List<JobMetadata>> GetAsync()
        {
            await Task.Delay(100);
            return FakeDB.Get();
        }

        public Task InsertAsync(JobMetadata jobMetadata)
        {
            FakeDB.Insert(jobMetadata);

            return Task.CompletedTask;
        }
    }
}