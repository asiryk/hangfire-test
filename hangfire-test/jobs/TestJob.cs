using Hangfire;
using HangfireTest.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace HangfireTest.Jobs;

public class TestJob : JobActivator
{

    private readonly ILogger logger;
    private readonly IHubContext<JobHub, IHubClient> hubContext;

    public TestJob(ILogger<TestJob> logger, IHubContext<JobHub, IHubClient> hubContext)
    {
        this.logger = logger;
        this.hubContext = hubContext;
    }

    public void GetAsync(string name)
    {
        var msg = string.Format("the job \"{0}\" has triggered", name);
        logger.LogInformation(msg);
        hubContext.Clients.All.NotifyClient(msg);
    }
}
