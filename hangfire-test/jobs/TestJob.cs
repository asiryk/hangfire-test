namespace HangfireDemo.Jobs;

public class TestJob
{

    private readonly ILogger logger;

    public TestJob(ILogger<TestJob> logger) => this.logger = logger;

    public void WriteLog(string logMessage)
    {
        logger.LogInformation($"{DateTime.Now:yyyy-MM-dd hh:mm:ss tt} {logMessage}");
    }

}
