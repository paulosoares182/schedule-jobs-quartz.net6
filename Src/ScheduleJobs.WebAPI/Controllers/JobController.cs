using Microsoft.AspNetCore.Mvc;

using ScheduleJobs.WebAPI.Jobs;
using ScheduleJobs.WebAPI.Jobs.Models;
using ScheduleJobs.WebAPI.Repository;
using ScheduleJobs.WebAPI.ViewModels;

namespace ScheduleJobs_Quartz.Net6.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class JobController : ControllerBase
    {
        private readonly ILogger<JobController> _logger;

        public JobController(ILogger<JobController> logger)
        {
            _logger = logger;
        }

        [HttpPost]

        public IActionResult Post(JobViewModel jobViewModel)
        {
            if (string.IsNullOrWhiteSpace(jobViewModel.CronExpression))
            {
                DateTime dt = jobViewModel.ScheduleDateTime;
                jobViewModel.CronExpression = $"0 {dt.Minute} {dt.Hour} {dt.Day} {dt.Month} ? {dt.Year}";
            }

            jobViewModel.Id = Guid.NewGuid().ToString();

            JobMetadata jobMetadata = new(jobViewModel.Id, typeof(MyJob), jobViewModel.Name, jobViewModel.CronExpression);
            FakeDB.Insert(jobMetadata);

            return Ok(jobViewModel);
        }

        [HttpGet]
        public IEnumerable<JobViewModel> Get()
        {
            List<JobViewModel> jobs = new();

            foreach(var job in FakeDB.Get())
            {
                jobs.Add(new JobViewModel
                {
                    Id = job.JobId,
                    Name = job.JobName,
                    CronExpression = job.CronExpression
                });
            }

            return jobs;
        }
    }
}