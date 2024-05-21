using Microsoft.AspNetCore.SignalR;

namespace HangfireTest.SignalR;

public interface IHubClient
{
    Task ReceiveMessage(string user, string message);
    Task NotifyClient(string message);
}

public class JobHub : Hub<IHubClient>
{

    private readonly ILogger logger;

    public JobHub(ILogger<JobHub> logger)
    {
        this.logger = logger;
    }
    public async Task SendMessage(string user, string message)
    {
        logger.LogInformation("client invocation with arguments {0}, {1}", user, message);
        await Clients.All.ReceiveMessage(user, message);
    }
}
