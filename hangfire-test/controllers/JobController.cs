using Hangfire;
using HangfireDemo.Jobs;
using Microsoft.AspNetCore.Mvc;

namespace HangfireDemo.AddControllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {

        private readonly IBackgroundJobClient backgroundJobs;
        private readonly IRecurringJobManager recurringJobs;

        public JobController(IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            backgroundJobs = backgroundJobClient;
            recurringJobs = recurringJobManager;
        }

        [HttpPost]
        [Route("CreateBackgroundJob")]
        public ActionResult CreateBackgroundJob()
        {
            Console.WriteLine("enter background job");
            backgroundJobs.Enqueue<TestJob>((x) => x.WriteLog("testJob: background job triggered"));
            backgroundJobs.Enqueue(() => Console.WriteLine("console: background job triggered"));
            return Ok();
        }

        [HttpPost]
        [Route("CreateScheduledJob")]
        public ActionResult CreateScheduledJob()
        {
            Console.WriteLine("enter scheduled job");
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);
            backgroundJobs.Schedule<TestJob>((x) => x.WriteLog("testJob: scheduled job triggered"), dateTimeOffset);
            backgroundJobs.Schedule(() => Console.WriteLine("console: scheduled job triggered"), dateTimeOffset);
            return Ok();
        }

        [HttpPost]
        [Route("CreateContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            Console.WriteLine("enter continuation job");
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

            var job1 = backgroundJobs.Schedule<TestJob>((x) => x.WriteLog("testJob: start job 1"), dateTimeOffset);
            var job2 = backgroundJobs.ContinueJobWith<TestJob>(job1, (x) => x.WriteLog("testJob: continue job 1"));
            var job3 = backgroundJobs.ContinueJobWith<TestJob>(job2, (x) => x.WriteLog("testJob: continue job 2"));
            var job4 = backgroundJobs.ContinueJobWith<TestJob>(job3, (x) => x.WriteLog("testJob: continue job 3"));

            var job1c = backgroundJobs.Schedule(() => Console.WriteLine("console: start job 1"), dateTimeOffset);
            var job2c = backgroundJobs.ContinueJobWith(job1, () => Console.WriteLine("console: continue job 1"));
            var job3c = backgroundJobs.ContinueJobWith(job2, () => Console.WriteLine("console: continue job 2"));
            var job4c = backgroundJobs.ContinueJobWith(job3, () => Console.WriteLine("console: continue job 3"));
            return Ok();
        }

        [HttpPost]
        [Route("CreateRecurringJob")]
        public ActionResult CreateRecurringJob()
        {
            recurringJobs.AddOrUpdate<TestJob>("RecurringJob1TestJob", (x) => x.WriteLog("testJob: recurring job triggered"), "* * * * *");
            recurringJobs.AddOrUpdate("RecurringJob1Console", () => Console.WriteLine("console: recurring job triggered"), "* * * * *");
            return Ok();
        }
    }

}
