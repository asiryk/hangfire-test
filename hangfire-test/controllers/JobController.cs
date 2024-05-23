using Hangfire;
using HangfireTest.Db;
using HangfireTest.Jobs;
using HangfireTest.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HangfireTest.Controller;

[Route("api/[controller]")]
[ApiController]
public class JobController : ControllerBase
{

    private readonly IBackgroundJobClient backgroundJobs;
    private readonly IRecurringJobManager recurringJobs;
    private readonly ILogger logger;
    private readonly IHubContext<JobHub, IHubClient> hubContext;
    private readonly TrackingDbContext dbContext;

    public JobController(
        IBackgroundJobClient backgroundJobClient,
        IRecurringJobManager recurringJobManager,
        ILogger<JobController> logger,
        IHubContext<JobHub, IHubClient> hubContext,
        TrackingDbContext dbContext
    )
    {
        backgroundJobs = backgroundJobClient;
        recurringJobs = recurringJobManager;
        this.logger = logger;
        this.hubContext = hubContext;
        this.dbContext = dbContext;
    }

    [HttpPost]
    [Route("CreateBackgroundJob")]
    public ActionResult CreateBackgroundJob()
    {
        logger.LogInformation("background job");
        notifyClient();
        backgroundJobs.Enqueue<TestJob>((x) => x.GetAsync("background"));
        return Ok();
    }

    [HttpPost]
    [Route("CreateScheduledJob")]
    public ActionResult CreateScheduledJob()
    {
        logger.LogInformation("scheduled job");
        var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
        var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

        backgroundJobs.Schedule<TestJob>((x) => x.GetAsync("scheduled"), dateTimeOffset);
        return Ok();
    }

    [HttpPost]
    [Route("CreateContinuationJob")]
    public ActionResult CreateContinuationJob()
    {
        logger.LogInformation("continuation job");
        var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
        var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

        var job1 = backgroundJobs.Schedule<TestJob>((x) => x.GetAsync("continuation/1"), dateTimeOffset);
        var job2 = backgroundJobs.ContinueJobWith<TestJob>(job1, (x) => x.GetAsync("continuation/2"));
        var job3 = backgroundJobs.ContinueJobWith<TestJob>(job2, (x) => x.GetAsync("continuation/3"));
        var job4 = backgroundJobs.ContinueJobWith<TestJob>(job3, (x) => x.GetAsync("continuation/4"));


        return Ok();
    }

    [HttpPost]
    [Route("CreateRecurringJob")]
    public ActionResult CreateRecurringJob()
    {
        logger.LogInformation("recurring job");
        recurringJobs.AddOrUpdate<TestJob>("RecurringJob1TestJob", (x) => x.GetAsync("recurring"), "* * * * *");
        return Ok();
    }


    [HttpPost]
    [Route("AddHealthCheck")]
    public async Task<ActionResult> AddHealthCheck(string serverId, string url)
    {
        var entry = new HealthCheckEntry
        {
            serverId = serverId,
            url = url,
        };

        logger.LogInformation("created entry {0}, {1}", entry.serverId, entry.url);

        await dbContext.HealthCheck.AddAsync(entry);
        await dbContext.SaveChangesAsync();
        return Ok("created entry");
    }

    private void notifyClient()
    {
        hubContext.Clients.All.NotifyClient("message from controller");
    }
}
