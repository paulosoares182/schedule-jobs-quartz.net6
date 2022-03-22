using Quartz;
using Quartz.Impl;
using Quartz.Spi;

using ScheduleJobs.WebAPI.Jobs;
using ScheduleJobs.WebAPI.Jobs.Factory;
using ScheduleJobs.WebAPI.Jobs.Schedular;
using ScheduleJobs.WebAPI.Repository;

using System.Collections.Specialized;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

NameValueCollection props = new()
{
    ["quartz.threadPool.threadCount"] = builder.Configuration.GetValue<string>("MaxThreadPool")
};

builder.Services.AddSingleton<IJobFactory, JobFactory>();
builder.Services.AddSingleton<ISchedulerFactory>(provider => new StdSchedulerFactory(props));
builder.Services.AddSingleton<MyJob>();
builder.Services.AddHostedService<JobScheduler>();

builder.Services.AddTransient<IJobRepository, JobRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
