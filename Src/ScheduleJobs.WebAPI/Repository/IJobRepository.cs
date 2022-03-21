using ScheduleJobs.WebAPI.Jobs.Models;

namespace ScheduleJobs.WebAPI.Repository
{
    public interface IJobRepository
    {
        Task<List<JobMetadata>> GetAsync();
        Task InsertAsync(JobMetadata jobMetadata);
    }
}