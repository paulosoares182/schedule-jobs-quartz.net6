using Quartz;
using Quartz.Impl;
using Quartz.Spi;

using ScheduleJobs.Worker;
using ScheduleJobs.Worker.Jobs;
using ScheduleJobs.Worker.Jobs.Factory;
using ScheduleJobs.Worker.Repository;

using System.Collections.Specialized;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        NameValueCollection props = new()
        {
            ["quartz.threadPool.threadCount"] = context.Configuration.GetValue<string>("MaxThreadPool")
        };

        services.AddSingleton<IJobFactory, JobFactory>();
        services.AddSingleton<ISchedulerFactory>(provider => new StdSchedulerFactory(props));
        services.AddSingleton<MyJob>();

        services.AddTransient<IJobRepository, JobRepository>();
        
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
