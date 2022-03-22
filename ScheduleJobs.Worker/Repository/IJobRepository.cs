using ScheduleJobs.Worker.Jobs.Models;

namespace ScheduleJobs.Worker.Repository
{
    public interface IJobRepository
    {
        Task<List<JobMetadata>> GetAsync();
        Task InsertAsync(JobMetadata jobMetadata);
    }
}