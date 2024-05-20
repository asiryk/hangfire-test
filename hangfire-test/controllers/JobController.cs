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
            backgroundJobs.Enqueue(() => new HttpClient().GetAsync("http://localhost:3000/background"));
            backgroundJobs.Enqueue<TestJob>((x) => x.GetAsync("background"));
            return Ok();
        }

        [HttpPost]
        [Route("CreateScheduledJob")]
        public ActionResult CreateScheduledJob()
        {
            Console.WriteLine("enter scheduled job");
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

            backgroundJobs.Schedule(() => new HttpClient().GetAsync("http://localhost:3000/scheduled"), dateTimeOffset);
            backgroundJobs.Schedule<TestJob>((x) => x.GetAsync("scheduled"), dateTimeOffset);
            return Ok();
        }

        [HttpPost]
        [Route("CreateContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            Console.WriteLine("enter continuation job");
            var scheduleDateTime = DateTime.UtcNow.AddSeconds(5);
            var dateTimeOffset = new DateTimeOffset(scheduleDateTime);

            {
                var job1 = backgroundJobs.Schedule(() => new HttpClient().GetAsync("http://localhost:3000/continuation/1"), dateTimeOffset);
                var job2 = backgroundJobs.ContinueJobWith(job1, () => new HttpClient().GetAsync("http://localhost:3000/continuation/2"));
                var job3 = backgroundJobs.ContinueJobWith(job2, () => new HttpClient().GetAsync("http://localhost:3000/continuation/3"));
                var job4 = backgroundJobs.ContinueJobWith(job3, () => new HttpClient().GetAsync("http://localhost:3000/continuation/4"));

            }

            {
                var job1 = backgroundJobs.Schedule<TestJob>((x) => x.GetAsync("continuation/1"), dateTimeOffset);
                var job2 = backgroundJobs.ContinueJobWith<TestJob>(job1, (x) => x.GetAsync("continuation/2"));
                var job3 = backgroundJobs.ContinueJobWith<TestJob>(job2, (x) => x.GetAsync("continuation/3"));
                var job4 = backgroundJobs.ContinueJobWith<TestJob>(job3, (x) => x.GetAsync("continuation/4"));
            }

            return Ok();
        }

        [HttpPost]
        [Route("CreateRecurringJob")]
        public ActionResult CreateRecurringJob()
        {
            recurringJobs.AddOrUpdate("RecurringJob1", () => new HttpClient().GetAsync("http://localhost:3000/recurring"), "* * * * *");
            recurringJobs.AddOrUpdate<TestJob>("RecurringJob1TestJob", (x) => x.GetAsync("recurring"), "* * * * *");
            return Ok();
        }
    }

}